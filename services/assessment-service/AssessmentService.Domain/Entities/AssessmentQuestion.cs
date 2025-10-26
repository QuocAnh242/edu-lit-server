using System;
using System.Collections.Generic;

namespace AssessmentService.Core.Entities
{
    public class AssessmentQuestion
    {
        public int AssessmentQuestionId { get; set; }
        public int AssessmentId { get; set; }
        public string QuestionId { get; set; }
        public int OrderNum { get; set; }
        public char CorrectAnswer { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Assessment Assessment { get; set; }
        public ICollection<AssessmentAnswer>? Answers { get; set; }
    }
}
