using AuthService.Application.Abstractions.Messaging;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.Abstractions.Messaging.Dispatcher;
using AuthService.Application.Users.Queries.GetUserById;
using AuthService.Application.Users.Queries.GetUserByUsername;
using AuthService.Application.Users.Queries.SearchUsers;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB for Read side
        services.AddSingleton<IMongoDatabase>(_ =>
        {
            var connectionString = configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017";
            var databaseName = configuration["Mongo:Database"] ?? "auth_query";
            
            var client = new MongoClient(connectionString);
            return client.GetDatabase(databaseName);
        });

        // MongoDB Read DAO and Repository
        services.AddScoped<IUserReadDAO, MongoUserReadDAO>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();

        // Query dispatcher
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserReadDto>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByUsernameQuery, UserReadDto>, GetUserByUsernameQueryHandler>();
        services.AddScoped<IQueryHandler<SearchUsersQuery, PagedResult<UserReadDto>>, SearchUsersQueryHandler>();

        // Background projector (RabbitMQ subscriber) - syncs from Write service to MongoDB
        services.AddHostedService<RabbitMqUserProjectionService>();
        
        //register other infrastructure services like email, logging, etc. here
        services.AddScoped<IMessageBusPublisher, RabbitMqPublisher>();

        return services;
    }
}