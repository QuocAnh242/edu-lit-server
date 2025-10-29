using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.Question.GetQuestionsByQuestionBankId
{
    public class GetQuestionsByQuestionBankIdQuery : IQuery<IEnumerable<QuestionDto>>
    {
        public Guid QuestionBankId { get; }

        public GetQuestionsByQuestionBankIdQuery(Guid questionBankId)
        {
            QuestionBankId = questionBankId;
        }
    }
}

