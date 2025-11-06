using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer
{
    public class UpdateAssessmentAnswerCommand : ICommand<bool>
    {
        public int AnswerId { get; set; }

        public int AssessmentQuestionId { get; set; }

        public int AttemptsId { get; set; }

        /// <summary>
        /// A, B, C, D mà student chọn
        /// </summary>
        public string SelectedAnswer { get; set; } = null!;

        public bool IsCorrect { get; set; }
    }
}
