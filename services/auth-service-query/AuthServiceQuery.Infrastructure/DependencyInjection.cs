using AuthService.Application.Abstractions.Messaging;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.DAO;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.Abstractions.Messaging.Dispatcher;
using AuthService.Application.Users.Queries.GetUserById;
using AuthService.Application.Users.Queries.GetUserByUsername;
using AuthService.Application.Users.Queries.SearchUsers;
using AuthService.Application.Users.Queries.GetUserRole;
using AuthService.Application.Roles.Queries.GetAllRoles;
using AuthService.Application.Roles.Queries.GetRoleById;
using AuthService.Application.Roles.Queries.GetRoleByName;
using AuthService.Application.Roles.Queries.SearchRoles;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure MongoDB GUID serialization
        try
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch (BsonSerializationException)
        {
            // Already registered, ignore
        }
        
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
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Query dispatcher
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Query handlers - Users
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserReadDto>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByUsernameQuery, UserReadDto>, GetUserByUsernameQueryHandler>();
        services.AddScoped<IQueryHandler<SearchUsersQuery, PagedResult<UserReadDto>>, SearchUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserRoleQuery, RoleDto>, GetUserRoleQueryHandler>();

        // Query handlers - Roles
        services.AddScoped<IQueryHandler<GetAllRolesQuery, List<RoleDto>>, GetAllRolesQueryHandler>();
        services.AddScoped<IQueryHandler<GetRoleByIdQuery, RoleDto>, GetRoleByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetRoleByNameQuery, RoleDto>, GetRoleByNameQueryHandler>();
        services.AddScoped<IQueryHandler<SearchRolesQuery, PagedResult<RoleDto>>, SearchRolesQueryHandler>();

        // Background projector (RabbitMQ subscriber) - syncs from Write service to MongoDB
        services.AddHostedService<RabbitMqUserProjectionService>();
        services.AddHostedService<RabbitMqRoleProjectionService>();
        
        //register other infrastructure services like email, logging, etc. here
        services.AddScoped<IMessageBusPublisher, RabbitMqPublisher>();

        return services;
    }
}