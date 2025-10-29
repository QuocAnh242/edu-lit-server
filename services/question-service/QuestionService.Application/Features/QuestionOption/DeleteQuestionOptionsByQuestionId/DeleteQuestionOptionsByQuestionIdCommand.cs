using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.QuestionOption.DeleteQuestionOptionsByQuestionId
{
    public class DeleteQuestionOptionsByQuestionIdCommand : ICommand<bool>
    {
        public Guid QuestionId { get; }

        public DeleteQuestionOptionsByQuestionIdCommand(Guid questionId)
        {
            QuestionId = questionId;
        }
    }
}



