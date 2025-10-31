using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.Question.DeleteQuestion
{
    public class DeleteQuestionCommand : ICommand<bool>
    {
        public Guid QuestionId { get; }

        public DeleteQuestionCommand(Guid questionId)
        {
            QuestionId = questionId;
        }
    }
}




