namespace AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswerByAttemptId
{
    public class GetAllAssessmentAnswerByAttemptIdResponse
    {
        public int AnswerId { get; set; }
        public int AssessmentQuestionId { get; set; }
        public int AttemptsId { get; set; }
        /// <summary>
        /// A, B, C, D mà student chọn
        /// </summary>
        public string SelectedAnswer { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
