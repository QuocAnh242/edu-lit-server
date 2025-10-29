using System.Reflection;
using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.Services.Role.Interfaces;
using AuthService.Application.Services.Users.Commands;
using AuthService.Application.Services.Users.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        // Dispatchers
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        // Handlers (explicit registrations)
        services.AddScoped<ICommandHandler<CreateUserCommand, Guid>, CreateUserHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, bool>, UpdateUserHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand, bool>, DeleteUserHandler>();

        // Application services
        services.AddScoped<IRoleService, Services.RoleService>();

        return services;
    }
}