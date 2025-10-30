using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.QuestionOption.GetQuestionOptionsByQuestionId
{
    public class GetQuestionOptionsByQuestionIdQuery : IQuery<IEnumerable<QuestionOptionDto>>
    {
        public Guid QuestionId { get; }

        public GetQuestionOptionsByQuestionIdQuery(Guid questionId)
        {
            QuestionId = questionId;
        }
    }
}




