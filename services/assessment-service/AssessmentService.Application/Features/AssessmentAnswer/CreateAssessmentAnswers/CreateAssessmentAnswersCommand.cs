using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswers
{
    public class CreateAssessmentAnswersCommand : ICommand<List<int>>
    {
        public int AttemptsId { get; set; }
        public List<Guid> SelectedOptionIds { get; set; } = new List<Guid>();
    }
}

