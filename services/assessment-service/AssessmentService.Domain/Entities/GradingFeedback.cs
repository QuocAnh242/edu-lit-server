using System;

namespace AssessmentService.Core.Entities
{
    public class GradingFeedback
    {
        public int FeedbackId { get; set; }
        public int AttemptsId { get; set; }
        public decimal TotalScore { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public decimal CorrectPercentage { get; set; }
        public decimal WrongPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public AssignmentAttempt AssignmentAttempt { get; set; }
    }
}
