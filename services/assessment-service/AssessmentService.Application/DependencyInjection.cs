using FluentValidation;
using AssessmentService.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AssessmentService.Application.Features.Assessment.CreateAssessment;
using AssessmentService.Application.Features.Assessment.GetAssessmentById;

namespace AssessmentService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services, handlers, and other dependencies here
            //Command
            services.AddScoped<ICommandHandler<CreateAssessmentCommand, int>, CreateAssessmentCommandHandler>();
            //Query
            services.AddScoped<IQueryHandler<GetAssessmentByIdQuery, GetAssessmentByIdResponse>, GetAssessmentByIdQueryHandler>();

            // Register all validators and AutoMapper profiles from the assembly
            //chỉ cần thêm dòng này là đủ để đăng ký tất cả validator và profile trong assembly ko cần đăng ký từng cái một
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //chỉ cần thêm dòng này là đủ để đăng ký tất cả AutoMapper profile trong assembly ko cần đăng ký từng cái một
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
