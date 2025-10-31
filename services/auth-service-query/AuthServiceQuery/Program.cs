using AuthService.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Infrastructure services (PostgreSQL Write DB + MongoDB Read DB + RabbitMQ)
builder.Services.AddAuthInfrastructure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Swagger + JWT security
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT. Example: Bearer {token}"
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, new List<string>() } });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
