using LessonService.Domain.IDAOs;
using LessonService.Domain.Interfaces;
using LessonService.Infrastructure.Persistance.DAOs;
using LessonService.Infrastructure.Persistance.DBContext;
using LessonService.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LessonService.Infrastructure
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

            // register db context
            services.AddDbContext<LessonDbContext>(option =>
            option.UseNpgsql(configuration.GetConnectionString("DefaultConnectionStringDB")));
            
            return services;
        }
    }
}
