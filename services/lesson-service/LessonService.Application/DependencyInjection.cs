using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LessonService.Application.Features.Syllabus.GetSyllabusById;

namespace LessonService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services, handlers, and other dependencies here
            //Command
            services.AddScoped<ICommandHandler<CreateSyllabusCommand, Guid>, CreateSyllabusCommandHandler>();
            //Query
            services.AddScoped<IQueryHandler<GetSyllabusByIdQuery, GetSyllabusByIdResponse>, GetSyllabusByIdQueryHandler>();

            // Register all validators and AutoMapper profiles from the assembly
            //chỉ cần thêm dòng này là đủ để đăng ký tất cả validator và profile trong assembly ko cần đăng ký từng cái một
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //chỉ cần thêm dòng này là đủ để đăng ký tất cả AutoMapper profile trong assembly ko cần đăng ký từng cái một
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
