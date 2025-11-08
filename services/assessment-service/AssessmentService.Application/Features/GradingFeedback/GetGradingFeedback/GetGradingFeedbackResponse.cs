namespace AssessmentService.Application.Features.GradingFeedback.GetGradingFeedback
{
    public class GetGradingFeedbackResponse
    {
        public int FeedbackId { get; set; }
        public int AttemptsId { get; set; }
        public decimal TotalScore { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public decimal CorrectPercentage { get; set; }
        public decimal WrongPercentage { get; set; }

        // ngoài ra thêm thông tin grade
        public string Grade { get; set; } = null!;
        
        // đánh giá chung
        public string Performance { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
