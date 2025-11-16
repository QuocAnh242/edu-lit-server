using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.GradingFeedback.CalculateGrading
{
    public class CalculateGradingCommand : ICommand<CalculateGradingResponse>
    {
        public int AttemptId { get; set; }
    }
}
