using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentAnswer.DeleteAssessmentAnswer
{
    public class DeleteAssessmentAnswerCommand : ICommand<bool>
    {
        public int AnswerId { get; }
        public DeleteAssessmentAnswerCommand(int answerId)
        {
            AnswerId = answerId;
        }
    }
}
