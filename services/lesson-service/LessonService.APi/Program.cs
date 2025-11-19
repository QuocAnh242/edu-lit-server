using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using LessonService.Application;
using LessonService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApiVersioning(options =>
{
    // Báo cáo các phiên bản API trong header "api-supported-versions"
    options.ReportApiVersions = true;
    // Tự động sử dụng phiên bản mặc định nếu client không chỉ định
    options.AssumeDefaultVersionWhenUnspecified = true;
    // Đặt phiên bản mặc định là 1.0
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
}).AddApiExplorer(options =>
{
    // Định dạng version trong URL, ví dụ: 'v'1.0
    options.GroupNameFormat = "'v'VVV";
    // Tự động thay thế tham số version trong route
    options.SubstituteApiVersionInUrl = true;
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Yêu cầu kiểm tra chữ ký của token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),

            // Yêu cầu kiểm tra nhà phát hành (Issuer)
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            // Yêu cầu kiểm tra đối tượng sử dụng (Audience)
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            // Yêu cầu kiểm tra thời gian sống của token
            ValidateLifetime = true,
            // Không cho phép có độ trễ thời gian khi token hết hạn
            ClockSkew = TimeSpan.Zero,

            // Chỉ định claim chứa thông tin về vai trò (Role)
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 1. Định nghĩa Security Scheme (Cách xác thực)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // 2. Áp dụng Security Scheme đó cho tất cả các endpoint cần xác thực
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<LessonService.Infrastructure.Persistance.DBContext.LessonDbContext>();
        
        logger.LogInformation("Checking database connection...");
        
        // Test connection first
        if (context.Database.CanConnect())
        {
            logger.LogInformation("✅ Database connection successful");
            logger.LogInformation("Applying database migrations...");
            context.Database.Migrate();
            logger.LogInformation("✅ Database migrations applied successfully");
        }
        else
        {
            logger.LogWarning("⚠️  Cannot connect to database. Migrations skipped.");
            logger.LogWarning("Make sure PostgreSQL is running:");
            logger.LogWarning("  - Local: docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=lesson_db --name postgres postgres:16");
            logger.LogWarning("  - Or: docker-compose up -d postgres_lesson");
        }
    }
    catch (System.Net.Sockets.SocketException ex)
    {
        logger.LogWarning(ex, "⚠️  Database host not reachable. Check your connection string:");
        logger.LogWarning("  - Development: Host should be 'localhost' (check appsettings.Development.json)");
        logger.LogWarning("  - Docker: Host should be 'postgres_lesson' (check appsettings.json)");
        logger.LogWarning("Application will start without migrations.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred while applying migrations");
        logger.LogWarning("Application will start without migrations.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must be before Authentication and Authorization
app.UseCors("AllowAll");

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
