using AssessmentService.Application.IServices;
using AssessmentService.Domain.IDAOs;
using AssessmentService.Domain.Interfaces;
using AssessmentService.Infrastructure.Persistance.DistributedCaches;
using AssessmentService.Infrastructure.Persistance.Repositories;
using AssessmentService.Infrastructure.Persistance.DAOs;
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
            
            // register services (unit of work)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //register third party
            services.AddScoped<IRedisService, RedisService>();


            var redisConfig = configuration.GetConnectionString("RedisConnection");

            var redisConnection = ConnectionMultiplexer.Connect(redisConfig);
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);

            //register redis for basic caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
                options.InstanceName = "assessment:";
            });
            
            return services;
        }
    }
}
