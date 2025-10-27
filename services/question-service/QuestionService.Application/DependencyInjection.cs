using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Application.Features.Question.CreateQuestion;
using QuestionService.Application.Features.Question.DeleteQuestion;
using QuestionService.Application.Features.Question.GetAllQuestions;
using QuestionService.Application.Features.Question.GetPublishedQuestions;
using QuestionService.Application.Features.Question.GetQuestionById;
using QuestionService.Application.Features.Question.GetQuestionsByAuthorId;
using QuestionService.Application.Features.Question.GetQuestionsByQuestionBankId;
using QuestionService.Application.Features.Question.GetQuestionsByType;
using QuestionService.Application.Features.Question.UpdateQuestion;
using QuestionService.Application.Features.QuestionBank.CreateQuestionBank;
using QuestionService.Application.Features.QuestionBank.DeleteQuestionBank;
using QuestionService.Application.Features.QuestionBank.GetAllQuestionBanks;
using QuestionService.Application.Features.QuestionBank.GetQuestionBankById;
using QuestionService.Application.Features.QuestionBank.GetQuestionBanksByOwnerId;
using QuestionService.Application.Features.QuestionBank.GetQuestionBanksBySubject;
using QuestionService.Application.Features.QuestionBank.UpdateQuestionBank;
using QuestionService.Application.Features.QuestionOption.CreateQuestionOption;
using QuestionService.Application.Features.QuestionOption.DeleteQuestionOption;
using QuestionService.Application.Features.QuestionOption.DeleteQuestionOptionsByQuestionId;
using QuestionService.Application.Features.QuestionOption.GetQuestionOptionById;
using QuestionService.Application.Features.QuestionOption.GetQuestionOptionsByQuestionId;
using QuestionService.Application.Features.QuestionOption.UpdateQuestionOption;
using System.Reflection;

namespace QuestionService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register Question Command Handlers
            services.AddScoped<ICommandHandler<CreateQuestionCommand, Guid>, CreateQuestionCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateQuestionCommand, Guid>, UpdateQuestionCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteQuestionCommand, bool>, DeleteQuestionCommandHandler>();

            // Register Question Query Handlers
            services.AddScoped<IQueryHandler<GetQuestionByIdQuery, QuestionDto>, GetQuestionByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllQuestionsQuery, IEnumerable<QuestionDto>>, GetAllQuestionsQueryHandler>();
            services.AddScoped<IQueryHandler<GetQuestionsByQuestionBankIdQuery, IEnumerable<QuestionDto>>, GetQuestionsByQuestionBankIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetQuestionsByAuthorIdQuery, IEnumerable<QuestionDto>>, GetQuestionsByAuthorIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetQuestionsByTypeQuery, IEnumerable<QuestionDto>>, GetQuestionsByTypeQueryHandler>();
            services.AddScoped<IQueryHandler<GetPublishedQuestionsQuery, IEnumerable<QuestionDto>>, GetPublishedQuestionsQueryHandler>();

            // Register QuestionBank Command Handlers
            services.AddScoped<ICommandHandler<CreateQuestionBankCommand, Guid>, CreateQuestionBankCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateQuestionBankCommand, Guid>, UpdateQuestionBankCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteQuestionBankCommand, bool>, DeleteQuestionBankCommandHandler>();

            // Register QuestionBank Query Handlers
            services.AddScoped<IQueryHandler<GetQuestionBankByIdQuery, QuestionBankDto>, GetQuestionBankByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllQuestionBanksQuery, IEnumerable<QuestionBankDto>>, GetAllQuestionBanksQueryHandler>();
            services.AddScoped<IQueryHandler<GetQuestionBanksByOwnerIdQuery, IEnumerable<QuestionBankDto>>, GetQuestionBanksByOwnerIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetQuestionBanksBySubjectQuery, IEnumerable<QuestionBankDto>>, GetQuestionBanksBySubjectQueryHandler>();

            // Register QuestionOption Command Handlers
            services.AddScoped<ICommandHandler<CreateQuestionOptionCommand, Guid>, CreateQuestionOptionCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateQuestionOptionCommand, Guid>, UpdateQuestionOptionCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteQuestionOptionCommand, bool>, DeleteQuestionOptionCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteQuestionOptionsByQuestionIdCommand, bool>, DeleteQuestionOptionsByQuestionIdCommandHandler>();

            // Register QuestionOption Query Handlers
            services.AddScoped<IQueryHandler<GetQuestionOptionByIdQuery, QuestionOptionDto>, GetQuestionOptionByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetQuestionOptionsByQuestionIdQuery, IEnumerable<QuestionOptionDto>>, GetQuestionOptionsByQuestionIdQueryHandler>();

            // Register all validators from the assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register AutoMapper with all profiles from the assembly
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
