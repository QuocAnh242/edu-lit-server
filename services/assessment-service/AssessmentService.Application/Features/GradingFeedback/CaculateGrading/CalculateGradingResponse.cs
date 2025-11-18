namespace AssessmentService.Application.Features.GradingFeedback.CalculateGrading
{
    public class CalculateGradingResponse
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

        // ngoài ra thêm thông tin grade
        public string Grade { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

    }
}
