using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion
{
    public class CreateAssessmentQuestionCommand : ICommand<int>
    {
        public int AssessmentId { get; set; }
        public Guid QuestionId { get; set; }
    }
}
