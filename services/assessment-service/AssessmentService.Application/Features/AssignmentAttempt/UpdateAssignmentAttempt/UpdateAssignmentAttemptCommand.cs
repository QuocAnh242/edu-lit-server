using AssessmentService.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace AssessmentService.Application.Features.AssignmentAttempt.UpdateAssignmentAttempt
{
    public class UpdateAssignmentAttemptCommand : ICommand<bool>
    {
        public int AttemptsId { get; set; }
        public int AssessmentId { get; set; }

        public string UserId { get; set; } = null!;

        /// ISO 8601 <example>2025-11-06T10:30:00Z</example>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedAt { get; set; }

        public int AttemptNumber { get; set; }

    }
}
