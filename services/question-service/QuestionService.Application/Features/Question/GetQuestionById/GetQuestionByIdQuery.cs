using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.Question.GetQuestionById
{
    public class GetQuestionByIdQuery : IQuery<QuestionDto>
    {
        public Guid QuestionId { get; }

        public GetQuestionByIdQuery(Guid questionId)
        {
            QuestionId = questionId;
        }
    }
}




