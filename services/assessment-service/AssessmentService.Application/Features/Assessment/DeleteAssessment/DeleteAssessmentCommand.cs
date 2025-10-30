using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.Assessment.DeleteAssessment
{
    public class DeleteAssessmentCommand : ICommand<bool>
    {
        public int AssessmentId { get; }
        public DeleteAssessmentCommand(int assessmentId)
        {
            AssessmentId = assessmentId;
        }
    }
}
