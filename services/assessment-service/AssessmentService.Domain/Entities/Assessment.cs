using System;
using System.Collections.Generic;

namespace AssessmentService.Domain.Entities;

public partial class Assessment
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

    public virtual ICollection<AssessmentQuestion> AssessmentQuestions { get; set; } = new List<AssessmentQuestion>();

    public virtual ICollection<AssignmentAttempt> AssignmentAttempts { get; set; } = new List<AssignmentAttempt>();
}
