using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.Assessment.CreateAssessment;
using AssessmentService.Application.Features.Assessment.DeleteAssessment;
using AssessmentService.Application.Features.Assessment.GetAllAssessment;
using AssessmentService.Application.Features.Assessment.GetAssessmentById;
using AssessmentService.Application.Features.Assessment.UpdateAssessment;
using AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.DeleteAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswerByAttemptId;
using AssessmentService.Application.Features.AssessmentAnswer.GetAssessmentAnswerById;
using AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.DeleteAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestionByAssessmentId;
using AssessmentService.Application.Features.AssessmentQuestion.GetAssessmentQuestionById;
using AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AssessmentService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services, handlers, and other dependencies here
            //Command
            services.AddScoped<ICommandHandler<CreateAssessmentCommand, int>, CreateAssessmentCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateAssessmentCommand, bool>, UpdateAssessmentCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteAssessmentCommand, bool>, DeleteAssessmentCommandHandler>();

            services.AddScoped<ICommandHandler<CreateAssessmentQuestionCommand, int>, CreateAssessmentQuestionCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateAssessmentQuestionCommand, bool>, UpdateAssessmentQuestionCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteAssessmentQuestionCommand, bool>, DeleteAssessmentQuestionCommandHandler>();

            services.AddScoped<ICommandHandler<CreateAssessmentAnswerCommand, int>, CreateAssessmentAnswerCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateAssessmentAnswerCommand, bool>, UpdateAssessmentAnswerCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteAssessmentAnswerCommand, bool>, DeleteAssessmentAnswerCommandHandler>();


            //Query
            services.AddScoped<IQueryHandler<GetAssessmentByIdQuery, GetAssessmentByIdResponse>, GetAssessmentByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllAssessmentQuery, List<GetAllAssessmentResponse>>, GetAllAssessmentQueryHandler>();

            services.AddScoped<IQueryHandler<GetAssessmentQuestionByIdQuery, GetAssessmentQuestionByIdResponse>, GetAssessmentQuestionByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllAssessmentQuestionQuery, List<GetAllAssessmentQuestionResponse>>, GetAllAssessmentQuestionQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllAssessmentQuestionByAssessmentIdQuery, List<GetAllAssessmentQuestionByAssessmentIdResponse>>, GetAllAssessmentQuestionByAssessmentIdQueryHandler>();

            services.AddScoped<IQueryHandler<GetAssessmentAnswerByIdQuery, GetAssessmentAnswerByIdResponse>, GetAssessmentAnswerByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllAssessmentAnswerQuery, List<GetAllAssessmentAnswerResponse>>, GetAllAssessmentAnswerQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllAssessmentAnswerByAttemptIdQuery, List<GetAllAssessmentAnswerByAttemptIdResponse>>, GetAllAssessmentAnswerByAttemptIdQueryHandler>();

            // Register all validators and AutoMapper profiles from the assembly
            //chỉ cần thêm dòng này là đủ để đăng ký tất cả validator và profile trong assembly ko cần đăng ký từng cái một
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //chỉ cần thêm dòng này là đủ để đăng ký tất cả AutoMapper profile trong assembly ko cần đăng ký từng cái một
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
