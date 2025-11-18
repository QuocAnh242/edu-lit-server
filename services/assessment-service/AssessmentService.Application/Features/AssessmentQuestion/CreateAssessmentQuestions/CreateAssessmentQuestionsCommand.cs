using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestions
{
    public class CreateAssessmentQuestionsCommand : ICommand<List<int>>
    {
        public int AssessmentId { get; set; }
        public List<Guid> QuestionIds { get; set; } = new List<Guid>();
    }
}

