using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.QuestionBank.GetAllQuestionBanks
{
    public class GetAllQuestionBanksQuery : IQuery<IEnumerable<QuestionBankDto>>
    {
    }
}



