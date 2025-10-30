using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.QuestionBank.GetQuestionBanksBySubject
{
    public class GetQuestionBanksBySubjectQuery : IQuery<IEnumerable<QuestionBankDto>>
    {
        public string Subject { get; }

        public GetQuestionBanksBySubjectQuery(string subject)
        {
            Subject = subject;
        }
    }
}



