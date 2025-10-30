using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.Question.GetPublishedQuestions
{
    public class GetPublishedQuestionsQuery : IQuery<IEnumerable<QuestionDto>>
    {
    }
}




