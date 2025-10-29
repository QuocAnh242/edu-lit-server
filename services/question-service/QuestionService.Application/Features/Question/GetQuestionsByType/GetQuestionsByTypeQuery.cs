using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Enums;

namespace QuestionService.Application.Features.Question.GetQuestionsByType
{
    public class GetQuestionsByTypeQuery : IQuery<IEnumerable<QuestionDto>>
    {
        public QuestionType QuestionType { get; }

        public GetQuestionsByTypeQuery(QuestionType questionType)
        {
            QuestionType = questionType;
        }
    }
}



