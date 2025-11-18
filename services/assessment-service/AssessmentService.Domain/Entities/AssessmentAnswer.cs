namespace AssessmentService.Domain.Entities;

public partial class AssessmentAnswer
{
    public int AnswerId { get; set; }

    public int AssessmentQuestionId { get; set; }

    public int AttemptsId { get; set; }

    /// <summary>
    /// Reference to QuestionOption in Question Service
    /// </summary>
    public string SelectedOptionId { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AssessmentQuestion AssessmentQuestion { get; set; } = null!;

    public virtual AssignmentAttempt Attempts { get; set; } = null!;
}
