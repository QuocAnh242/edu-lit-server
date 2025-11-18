using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Abstractions.Messaging.Dispatcher;
using LessonService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonService.Application.Features.Activities.CreateActivity;
using LessonService.Application.Features.Activities.DeleteActivity;
using LessonService.Application.Features.Activities.UpdateActivity;
using LessonService.Application.Features.Courses.CreateCourse;
using LessonService.Application.Features.Courses.DeleteCourse;
using LessonService.Application.Features.Courses.UpdateCourse;
using LessonService.Application.Features.LessonContexts.CreateBulkLessonContexts;
using LessonService.Application.Features.LessonContexts.CreateLessonContext;
using LessonService.Application.Features.LessonContexts.DeleteLessonContext;
using LessonService.Application.Features.LessonContexts.UpdateLessonContext;
using LessonService.Application.Features.Sessions.CreateSession;
using LessonService.Application.Features.Sessions.DeleteSession;
using LessonService.Application.Features.Sessions.UpdateSession;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using LessonService.Application.Features.Syllabus.UpdateSyllabus;
using LessonService.Application.Features.Syllabus.DeleteSyllabus;


namespace LessonService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services, handlers, and other dependencies here
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();

            // Syllabus Handlers
            services.AddScoped<ICommandHandler<CreateSyllabusCommand, Guid>, CreateSyllabusCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateSyllabusCommand>, UpdateSyllabusCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteSyllabusCommand>, DeleteSyllabusCommandHandler>();

            // Activity Handlers
            services.AddScoped<ICommandHandler<CreateActivityCommand, Guid>, CreateActivityCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateActivityCommand>, UpdateActivityCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteActivityCommand>, DeleteActivityCommandHandler>();
            
            // Course Handlers
            services.AddScoped<ICommandHandler<CreateCourseCommand, Guid>, CreateCourseCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateCourseCommand>, UpdateCourseCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteCourseCommand>, DeleteCourseCommandHandler>();
            
            // Session Handlers
            services.AddScoped<ICommandHandler<CreateSessionCommand, Guid>, CreateSessionCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateSessionCommand>, UpdateSessionCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteSessionCommand>, DeleteSessionCommandHandler>();
            
            // LessonContext Handlers
            services.AddScoped<ICommandHandler<CreateLessonContextCommand, Guid>, CreateLessonContextCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBulkLessonContextsCommand, List<Guid>>, CreateBulkLessonContextsCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateLessonContextCommand>, UpdateLessonContextCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteLessonContextCommand>, DeleteLessonContextCommandHandler>();
            
            // Register all validators and AutoMapper profiles from the assembly
            //chỉ cần thêm dòng này là đủ để đăng ký tất cả validator và profile trong assembly ko cần đăng ký từng cái một
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //chỉ cần thêm dòng này là đủ để đăng ký tất cả AutoMapper profile trong assembly ko cần đăng ký từng cái một
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
