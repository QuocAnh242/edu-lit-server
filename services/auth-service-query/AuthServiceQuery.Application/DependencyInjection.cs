using System.Reflection;
using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        // Query Dispatcher (registered in Infrastructure layer with Query Handlers)
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        // This layer is for Application-specific services only
        
        return services;
    }
}