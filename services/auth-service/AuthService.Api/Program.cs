using AuthService.Application;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Enums;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Middlewares;
using AuthService.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Handling ERROR 400 (model validation) to ApiResponse
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var firstError = context.ModelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .Select(kvp => $"{kvp.Key}: {kvp.Value!.Errors.First().ErrorMessage}")
            .FirstOrDefault() ?? "Missing/Invalid input";
        var body = ApiResponse<string>.Error(ApiStatusCode.HB40001, firstError);
        return new BadRequestObjectResult(body);
    };
});

builder.Services.AddEndpointsApiExplorer();
// Lowercase URLs for apis
builder.Services.AddRouting(options => options.LowercaseUrls = true);
// Add RabbitMQ publisher (single registration)
builder.Services.AddSingleton<IMessageBusPublisher, RabbitMqPublisher>();
    
// Swagger + JWT security
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Application and Infrastructure layers
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddAuthApplication();

// Cors setup
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// JWT setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
        RoleClaimType = ClaimTypes.Role
    };
    
    // Customize authentication failure response
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var auth = ctx.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(auth))
            {
                if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    ctx.Token = auth.Substring("Bearer ".Length).Trim();
                else
                    ctx.Token = auth.Trim(); // raw token
            }
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(
                ApiResponse<string>.Error(ApiStatusCode.HB40101, "Token missing/Invalid"));
            await context.Response.WriteAsync(payload);
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(
                ApiResponse<string>.Error(ApiStatusCode.HB40301, "Permission Denied"));
            await context.Response.WriteAsync(payload);
        }
    };
});

var app = builder.Build();

// Apply database migrations automatically on startup
// This MUST complete successfully before the app starts
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthService.Infrastructure.Data.AuthDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Starting database migration...");
    
    try
    {
        // Apply pending migrations (this will create the database if it doesn't exist)
        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations: {Migrations}", 
                pendingMigrations.Count, string.Join(", ", pendingMigrations));
            dbContext.Database.Migrate();
            logger.LogInformation("Database migration completed successfully.");
        }
        else
        {
            logger.LogInformation("Database is up to date. No pending migrations.");
        }

               // Seed default roles if they don't exist
               await SeedDefaultRolesAsync(scope, logger);
               
               // Wait a bit for roles to be published and synced to MongoDB
               // This ensures roles are available in query service before accepting requests
               logger.LogInformation("Waiting for roles to be synced to query service...");
               await Task.Delay(TimeSpan.FromSeconds(3), CancellationToken.None);
               logger.LogInformation("Role seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "CRITICAL: Database migration failed. Application will not start.");
        // In production, you might want to retry or exit
        // For now, we'll throw to prevent the app from starting with a broken database
        throw;
    }
}

// Helper method to seed default roles
static async Task SeedDefaultRolesAsync(AsyncServiceScope scope, ILogger logger)
{
    try
    {
        var roleRepository = scope.ServiceProvider.GetRequiredService<AuthService.Domain.Interfaces.IRoleRepository>();
        var commandDispatcher = scope.ServiceProvider.GetRequiredService<AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces.ICommandDispatcher>();
        
        var defaultRoles = new[] { "STUDENT", "TEACHER", "ADMIN" };
        
        foreach (var roleName in defaultRoles)
        {
            var existingRole = await roleRepository.GetByNameAsync(roleName);
            if (existingRole == null)
            {
                logger.LogInformation("Creating default role: {RoleName}", roleName);
                var createRoleCmd = new AuthService.Application.Services.Role.Commands.CreateRoleCommand { Name = roleName };
                var result = await commandDispatcher.Send<AuthService.Application.Services.Role.Commands.CreateRoleCommand, Guid>(createRoleCmd, CancellationToken.None);
                
                if (result.Success)
                {
                    logger.LogInformation("Successfully created role: {RoleName}", roleName);
                }
                else
                {
                    logger.LogWarning("Failed to create role {RoleName}: {Error}", roleName, result.Message);
                }
            }
            else
            {
                logger.LogDebug("Role {RoleName} already exists", roleName);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding default roles. Continuing anyway...");
        // Don't throw - roles might already exist
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Only redirect to HTTPS in Production
if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Global error handler (maps exceptions to ApiResponse with proper HTTP status)
app.UseMiddleware<ErrorHandlingMiddleware>();

// For non-exception 404s (no matching endpoint), return ApiResponse
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == StatusCodes.Status404NotFound &&
        !response.HasStarted)
    {
        response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(
            ApiResponse<string>.Error(ApiStatusCode.HB40401, "Resource not found"));
        await response.WriteAsync(payload);
    }
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
