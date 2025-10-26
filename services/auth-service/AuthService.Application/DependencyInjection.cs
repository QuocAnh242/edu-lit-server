using System.Reflection;
using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Services.Interfaces;
using AuthService.Application.Users.Commands;
using AuthService.Application.Users.Handlers;
using AuthService.Application.Users.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        // Dispatchers
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Handlers (explicit registrations)
        services.AddScoped<IQueryHandler<GetUsersQuery, List<DTOs.UserDto>>, GetUsersHandler>();
        services.AddScoped<IQueryHandler<GetUserByIdQuery, DTOs.UserDto>, GetUserByIdHandler>();
        services.AddScoped<IQueryHandler<GetUsersPagedQuery, DTOs.Response.PageResponse<DTOs.UserDto>>, GetUsersPagedHandler>();
        services.AddScoped<ICommandHandler<CreateUserCommand, Guid>, CreateUserHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, bool>, UpdateUserHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand, bool>, DeleteUserHandler>();

        // Application services
        services.AddScoped<IAuthService, Services.AuthService>();
        services.AddScoped<IUserService, Services.UserService>();
        services.AddScoped<IRoleService, Services.RoleService>();

        return services;
    }
}