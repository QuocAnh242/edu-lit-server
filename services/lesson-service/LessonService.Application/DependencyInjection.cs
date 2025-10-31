﻿using FluentValidation;
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
using LessonService.Application.Features.Syllabus.DeleteSyllabus;
using LessonService.Application.Features.Syllabus.GetPagination;
using LessonService.Application.Features.Syllabus.GetPaginationSyllabus;
using LessonService.Application.Features.Syllabus.GetSyllabusById;
using LessonService.Application.Features.Syllabus.UpdateSyllabus;
using LessonService.Application.Features.Courses.CreateCourse;
using LessonService.Application.Features.Courses.UpdateCourse;
using LessonService.Application.Features.Courses.DeleteCourse;
using LessonService.Application.Features.Courses.GetCourseById;
using LessonService.Application.Features.Courses.GetPaginationCourses;
using LessonService.Application.Features.Sessions.CreateSession;
using LessonService.Application.Features.Sessions.UpdateSession;
using LessonService.Application.Features.Sessions.DeleteSession;
using LessonService.Application.Features.Sessions.GetSessionById;
using LessonService.Application.Features.Sessions.GetPaginationSessions;
using LessonService.Application.Features.Activities.CreateActivity;
using LessonService.Application.Features.Activities.UpdateActivity;
using LessonService.Application.Features.Activities.DeleteActivity;
using LessonService.Application.Features.Activities.GetActivityById;
using LessonService.Application.Features.Activities.GetPaginationActivities;
using LessonService.Application.Features.LessonContexts.CreateLessonContext;
using LessonService.Application.Features.LessonContexts.UpdateLessonContext;
using LessonService.Application.Features.LessonContexts.DeleteLessonContext;
using LessonService.Application.Features.LessonContexts.GetLessonContextById;
using LessonService.Application.Features.LessonContexts.GetPaginationLessonContexts;
using LessonService.Domain.Commons;

namespace LessonService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services, handlers, and other dependencies here
            //Syllabus Commands
            services.AddScoped<ICommandHandler<CreateSyllabusCommand, Guid>, CreateSyllabusCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateSyllabusCommand>, UpdateSyllabusCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteSyllabusCommand>, DeleteSyllabusCommandHandler>();
            
            //Syllabus Queries
            services.AddScoped<IQueryHandler<GetSyllabusByIdQuery, GetSyllabusByIdResponse>, GetSyllabusByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetSyllabusesQuery, PagedResult<GetSyllabusesResponse>>, GetSyllabusesQueryHandler>();

            //Course Commands
            services.AddScoped<ICommandHandler<CreateCourseCommand, Guid>, CreateCourseCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateCourseCommand>, UpdateCourseCommandHandler>();
            //Activity Commands
            services.AddScoped<ICommandHandler<CreateActivityCommand, Guid>, CreateActivityCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateActivityCommand>, UpdateActivityCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteActivityCommand>, DeleteActivityCommandHandler>();
            
            //Activity Queries
            services.AddScoped<IQueryHandler<GetActivityByIdQuery, GetActivityByIdResponse>, GetActivityByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetActivitiesQuery, PagedResult<GetActivitiesResponse>>, GetActivitiesQueryHandler>();

            //LessonContext Commands
            services.AddScoped<ICommandHandler<CreateLessonContextCommand, Guid>, CreateLessonContextCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateLessonContextCommand>, UpdateLessonContextCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteLessonContextCommand>, DeleteLessonContextCommandHandler>();
            
            //LessonContext Queries
            services.AddScoped<IQueryHandler<GetLessonContextByIdQuery, GetLessonContextByIdResponse>, GetLessonContextByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetLessonContextsQuery, PagedResult<GetLessonContextsResponse>>, GetLessonContextsQueryHandler>();

            services.AddScoped<ICommandHandler<DeleteCourseCommand>, DeleteCourseCommandHandler>();
            //Session Commands
            services.AddScoped<ICommandHandler<CreateSessionCommand, Guid>, CreateSessionCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateSessionCommand>, UpdateSessionCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteSessionCommand>, DeleteSessionCommandHandler>();
            
            //Session Queries
            services.AddScoped<IQueryHandler<GetSessionByIdQuery, GetSessionByIdResponse>, GetSessionByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetSessionsQuery, PagedResult<GetSessionsResponse>>, GetSessionsQueryHandler>();

            
            //Course Queries
            services.AddScoped<IQueryHandler<GetCourseByIdQuery, GetCourseByIdResponse>, GetCourseByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetCoursesQuery, PagedResult<GetCoursesResponse>>, GetCoursesQueryHandler>();

            // Register all validators and AutoMapper profiles from the assembly
            //chỉ cần thêm dòng này là đủ để đăng ký tất cả validator và profile trong assembly ko cần đăng ký từng cái một
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //chỉ cần thêm dòng này là đủ để đăng ký tất cả AutoMapper profile trong assembly ko cần đăng ký từng cái một
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
