using System;
using System.Collections.Generic;

namespace AssessmentService.Database.Models;

public partial class AssessmentAnswer
{
    public int AnswerId { get; set; }

    public int AssessmentQuestionId { get; set; }

    public int AttemptsId { get; set; }

    /// <summary>
    /// A, B, C, D mà student chọn
    /// </summary>
    public string SelectedAnswer { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AssessmentQuestion AssessmentQuestion { get; set; } = null!;

    public virtual AssignmentAttempt Attempts { get; set; } = null!;
}
