using System;
using System.Collections.Generic;

namespace AssessmentService.Database.Models;

public partial class AssignmentAttempt
{
    public int AttemptsId { get; set; }

    public int AssessmentId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int AttemptNumber { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual ICollection<AssessmentAnswer> AssessmentAnswers { get; set; } = new List<AssessmentAnswer>();

    public virtual GradingFeedback? GradingFeedback { get; set; }
}
