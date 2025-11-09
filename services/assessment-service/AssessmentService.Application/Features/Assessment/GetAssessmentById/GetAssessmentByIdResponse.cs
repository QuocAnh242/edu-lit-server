namespace AssessmentService.Application.Features.Assessment.GetAssessmentById
{
    public class GetAssessmentByIdResponse
    {
        public int AssessmentId { get; set; }

        public string CourseId { get; set; } = null!;

        public string CreatorId { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationMinutes { get; set; }

        public string? Status { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
