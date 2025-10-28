using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.Assessment.CreateAssessment
{
    public class CreateAssessmentCommand : ICommand<int>
    {
        public string CourseId { get; set; } = null!;
        public string CreatorId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
    }
}
