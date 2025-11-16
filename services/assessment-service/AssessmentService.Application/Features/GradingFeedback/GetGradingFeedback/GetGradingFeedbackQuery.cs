using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.GradingFeedback.GetGradingFeedback
{
    public class GetGradingFeedbackQuery : IQuery<GetGradingFeedbackResponse>
    {
        public int Id { get; set; }
        public GetGradingFeedbackQuery(int id)
        {
            Id = id;
        }
    }
}
