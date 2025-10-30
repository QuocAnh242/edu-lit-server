using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Features.QuestionBank.GetQuestionBanksByOwnerId
{
    public class GetQuestionBanksByOwnerIdQuery : IQuery<IEnumerable<QuestionBankDto>>
    {
        public Guid OwnerId { get; }

        public GetQuestionBanksByOwnerIdQuery(Guid ownerId)
        {
            OwnerId = ownerId;
        }
    }
}



