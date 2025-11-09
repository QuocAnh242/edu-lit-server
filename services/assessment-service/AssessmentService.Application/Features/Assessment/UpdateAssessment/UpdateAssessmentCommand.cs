using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.Assessment.UpdateAssessment
{
    public class UpdateAssessmentCommand : ICommand<bool>
    {
        public int Id { get; set; }
        public string CourseId { get; set; } = null!;
        public string CreatorId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
    }
}
