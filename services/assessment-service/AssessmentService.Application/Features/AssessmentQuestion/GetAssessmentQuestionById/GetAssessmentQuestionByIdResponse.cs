namespace AssessmentService.Application.Features.AssessmentQuestion.GetAssessmentQuestionById
{
    public class GetAssessmentQuestionByIdResponse
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

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
