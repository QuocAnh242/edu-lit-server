using QuestionService.Application.Abstractions.Messaging;

namespace QuestionService.Application.Features.QuestionOption.DeleteQuestionOption
{
    public class DeleteQuestionOptionCommand : ICommand<bool>
    {
        public Guid QuestionOptionId { get; }

        public DeleteQuestionOptionCommand(Guid questionOptionId)
        {
            QuestionOptionId = questionOptionId;
        }
    }
}

