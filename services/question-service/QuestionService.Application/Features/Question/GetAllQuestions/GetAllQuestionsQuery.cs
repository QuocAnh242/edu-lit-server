using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.Question.GetAllQuestions
{
    public class GetAllQuestionsQuery : IQuery<IEnumerable<QuestionDto>>
    {
    }
}



