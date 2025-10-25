using System;
using System.Collections.Generic;

namespace AssessmentService.Core.Entities
{
    public class AssignmentAttempt
    {
        public int AttemptsId { get; set; }
        public int AssessmentId { get; set; }
        public string UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Assessment Assessment { get; set; }
        public ICollection<AssessmentAnswer>? Answers { get; set; }
        public GradingFeedback? Feedback { get; set; }
    }
}
