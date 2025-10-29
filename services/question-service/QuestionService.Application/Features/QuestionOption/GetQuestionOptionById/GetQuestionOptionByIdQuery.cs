using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.QuestionOption.GetQuestionOptionById
{
    public class GetQuestionOptionByIdQuery : IQuery<QuestionOptionDto>
    {
        public Guid QuestionOptionId { get; }

        public GetQuestionOptionByIdQuery(Guid questionOptionId)
        {
            QuestionOptionId = questionOptionId;
        }
    }
}



