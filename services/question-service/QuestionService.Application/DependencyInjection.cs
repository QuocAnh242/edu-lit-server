using Microsoft.Extensions.DependencyInjection;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Interfaces;

namespace QuestionService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IQuestionService, QuestionService.Application.Services.QuestionService>();
            services.AddScoped<IQuestionBankService, QuestionBankService>();
            services.AddScoped<IQuestionOptionService, QuestionOptionService>();

            return services;
        }
    }
}


