using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Domain.Repositories;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Persistance.DAOs;
using LessonServiceQuery.Infrastructure.Persistance.Repositories;
using LessonServiceQuery.Infrastructure.Services;
using LessonServiceQuery.Infrastructure.Services.Consumers;
using LessonServiceQuery.Infrastructure.Services.EventHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LessonServiceQuery.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        
        //rabbitmq configuration
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));
        
        // MongoDB Context
        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        
        // DAOs
        services.AddScoped<ILessonDao, LessonDao>();
        services.AddScoped<ILessonContextDao, LessonContextDao>();
        services.AddScoped<IActivityDao, ActivityDao>();
        services.AddScoped<ISyllabusDao, SyllabusDao>();
        services.AddScoped<ICourseDao, CourseDao>();
        services.AddScoped<ISessionDao, SessionDao>();
        
        // Repositories
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<ISyllabusRepository, SyllabusRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        
        // Universal Event Handler
        services.AddScoped<UniversalEventHandler>();
        
        // RabbitMQ Universal Consumer (Hosted Service)
        services.AddHostedService<UniversalEventConsumer>();
        
        return services;
    }
}
