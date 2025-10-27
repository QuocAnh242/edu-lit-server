using System;
using System.Collections.Generic;

namespace AssessmentService.Domain.Entities;

public partial class AssessmentQuestion
{
    public int AssessmentQuestionId { get; set; }

    public int AssessmentId { get; set; }

    public string QuestionId { get; set; } = null!;

    public int OrderNum { get; set; }

    /// <summary>
    /// A, B, C, D
    /// </summary>
    public string CorrectAnswer { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual ICollection<AssessmentAnswer> AssessmentAnswers { get; set; } = new List<AssessmentAnswer>();
}
