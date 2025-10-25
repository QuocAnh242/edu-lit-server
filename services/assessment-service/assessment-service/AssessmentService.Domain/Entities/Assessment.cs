using System;
using System.Collections.Generic;

namespace AssessmentService.Core.Entities
{
    public class Assessment
    {
        public int AssessmentId { get; set; }
        public string CourseId { get; set; }
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<AssessmentQuestion>? Questions { get; set; }
        public ICollection<AssignmentAttempt>? Attempts { get; set; }
    }
}
