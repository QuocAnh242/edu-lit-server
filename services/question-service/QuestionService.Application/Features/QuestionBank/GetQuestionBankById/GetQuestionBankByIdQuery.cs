using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.QuestionBank.GetQuestionBankById
{
    public class GetQuestionBankByIdQuery : IQuery<QuestionBankDto>
    {
        public Guid QuestionBankId { get; }

        public GetQuestionBankByIdQuery(Guid questionBankId)
        {
            QuestionBankId = questionBankId;
        }
    }
}




