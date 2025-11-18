using LessonServiceQuery.Application.Abstractions.Messaging.Dispatcher;
using LessonServiceQuery.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using LessonServiceQuery.Application.Abstractions.Messaging;

namespace LessonServiceQuery.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        // Register Query Dispatcher
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        // Register AutoMapper
        services.AddAutoMapper(assembly);
        // Register all query handlers
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
    }
}