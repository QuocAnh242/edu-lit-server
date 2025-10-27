using System;
using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Entities.ReadModels;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.DAO.Mongo;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.JWT;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Read;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core DbContext (PostgreSQL)
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // DAOs (write side - PostgreSQL)
        services.AddScoped<IAuthDAO, AuthDAO>();
        services.AddScoped<IUserDAO, UserDAO>();
        services.AddScoped<IRoleDAO, RoleDAO>();

        // Repositories (write side)
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // JWT token generator
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Outbox + background publisher
        services.AddScoped<IOutbox, EfCoreOutbox>();
        services.AddHostedService<OutboxPublisherBackgroundService>();

        // Remove IConnection singleton; register publisher directly.
        services.AddSingleton<IMessageBusPublisher, RabbitMqPublisher>();

        // MongoDB (read side)
        services.AddSingleton<IMongoClient>(sp =>
        {
            var cs = configuration.GetSection("Mongo")["ConnectionString"] ?? "mongodb://localhost:27017";
            return new MongoClient(cs);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var dbName = configuration.GetSection("Mongo")["Database"] ?? "auth_read";
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(dbName);
        });

        services.AddSingleton<IMongoCollection<UserReadModel>>(sp =>
        {
            var db = sp.GetRequiredService<IMongoDatabase>();
            return db.GetCollection<UserReadModel>("users");
        });

        // Read-side DAO/Repo + projection (Mongo)
        services.AddScoped<IUserReadDAO, UserReadDAO>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IUserReadProjection, MongoUserReadProjection>();

        return services;
    }
}