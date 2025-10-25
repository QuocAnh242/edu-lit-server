using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Interfaces;
using QuestionService.Infrastructure.Data;
using QuestionService.Infrastructure.Persistance.DAOs;
using QuestionService.Infrastructure.Persistance.Repositories;
using QuestionService.Domain.IDAOs;
using QuestionService.Domain.IRepositories;
using QuestionService.Infrastructure.Repositories;

namespace QuestionService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<QuestionDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericDAO<>), typeof(GenericDAO<>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuestionBankRepository, QuestionBankRepository>();
            services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();

            return services;
        }
    }
}


