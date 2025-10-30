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
using MongoDB.Driver;
using RabbitMQ.Client;
using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.Abstractions.Messaging.Dispatcher;
using AuthService.Application.Users.Queries.GetUserById;
using AuthService.Application.Users.Queries.GetUserByUsername;
using AuthService.Application.Users.Queries.SearchUsers;

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

        // EF Core DbContext (ReadDb - PostgreSQL)
        services.AddDbContext<ReadDbContext>(opt =>
        {
            var cs = configuration.GetConnectionString("ReadDb")
                     ?? configuration.GetConnectionString("DefaultConnection");
            opt.UseNpgsql(cs);
        });

        // Query dispatcher
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetUserByIdQuery, AuthService.Application.DTOs.UserReadDto>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByUsernameQuery, AuthService.Application.DTOs.UserReadDto>, GetUserByUsernameQueryHandler>();
        services.AddScoped<IQueryHandler<SearchUsersQuery, AuthService.Application.DTOs.Response.PagedResult<AuthService.Application.DTOs.UserReadDto>>, SearchUsersQueryHandler>();

        // Background projector (RabbitMQ subscriber)
        services.AddHostedService<RabbitMqUserProjectionService>();

        return services;
    }
}