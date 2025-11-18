namespace AssessmentService.Application.Features.AssignmentAttempt.GetAllAssigmentAttempt
{
    public class GetAllAssignmentAttemptResponse
    {
        public int AttemptsId { get; set; }

        public int AssessmentId { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int AttemptNumber { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
