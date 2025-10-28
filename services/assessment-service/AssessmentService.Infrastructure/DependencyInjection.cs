using AssessmentService.Application.IServices;
using AssessmentService.Domain.IDAOs;
using AssessmentService.Domain.Interfaces;
using AssessmentService.Infrastructure.Persistance.DistributedCaches;
using AssessmentService.Infrastructure.Persistance.Repositories;
using AssessmentService.Infrastructure.Persistance.DAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AssessmentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //register other infrastructure services
            services.AddScoped(typeof(IGenericDAO<>), typeof(GenericDAO<>));

            //register repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //register third party
            services.AddScoped<IRedisService, RedisService>();
            
            // register services (unit of work)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // register db context
            /*services.AddDbContext<AssessmentDbContext>(options => 
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));*/
            
            var redisConfig = configuration.GetConnectionString("Redis");
            
            //register redis for basic caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
                options.InstanceName = "LessonService_";
            });
            
            //register redis for advanced caching\
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConfig));
            
            return services;
        }
    }
}
