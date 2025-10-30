using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.QuestionBank.DeleteQuestionBank
{
    public class DeleteQuestionBankCommand : ICommand<bool>
    {
        public Guid QuestionBankId { get; }

        public DeleteQuestionBankCommand(Guid questionBankId)
        {
            QuestionBankId = questionBankId;
        }
    }
}



