using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using LessonServiceQuery.Application;
using LessonServiceQuery.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

// Add Infrastructure services
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
