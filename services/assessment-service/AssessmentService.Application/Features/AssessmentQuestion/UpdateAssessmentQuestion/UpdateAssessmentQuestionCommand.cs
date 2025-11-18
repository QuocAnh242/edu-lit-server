using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion
{
    public class UpdateAssessmentQuestionCommand : ICommand<bool>
    {
        public int AssessmentQuestionId { get; set; }
        public int AssessmentId { get; set; }
        public Guid QuestionId { get; set; }
        public bool? IsActive { get; set; }
    }
}
