using System;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.JWT;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using AuthService.Application.Abstractions.Messaging;

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

        return services;
    }
}