using System;

namespace AssessmentService.Core.Entities
{
    public class AssessmentAnswer
    {
        public int AnswerId { get; set; }
        public int AssessmentQuestionId { get; set; }
        public int AttemptsId { get; set; }
        public char SelectedAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime CreatedAt { get; set; }

        public AssessmentQuestion AssessmentQuestion { get; set; }
        public AssignmentAttempt AssignmentAttempt { get; set; }
    }
}
