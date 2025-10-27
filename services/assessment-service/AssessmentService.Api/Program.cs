using AssessmentService.Application.Services;
using AssessmentService.Application.Services.Interfaces;
using AssessmentService.Domain.Interfaces;
using AssessmentService.Infrastructure.DAO;
using AssessmentService.Infrastructure.Data;
using AssessmentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AssessmentDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(5, 7, 43))
    ));

// Register DAOs (scoped)
builder.Services.AddScoped<AssessmentDAO>();

// Register DI services and repositories
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IAssessmentService, AssessmentServices>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
