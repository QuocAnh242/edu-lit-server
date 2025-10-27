using System;
using System.Collections.Generic;

namespace AssessmentService.Domain.Entities;

public partial class GradingFeedback
{
    public int FeedbackId { get; set; }

    public int AttemptsId { get; set; }

    /// <summary>
    /// Tổng điểm trên thang 10
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// Số câu trả lời đúng
    /// </summary>
    public int CorrectCount { get; set; }

    /// <summary>
    /// Số câu trả lời sai
    /// </summary>
    public int WrongCount { get; set; }

    /// <summary>
    /// Phần trăm câu đúng (%)
    /// </summary>
    public decimal CorrectPercentage { get; set; }

    /// <summary>
    /// Phần trăm câu sai (%)
    /// </summary>
    public decimal WrongPercentage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AssignmentAttempt Attempts { get; set; } = null!;
}
