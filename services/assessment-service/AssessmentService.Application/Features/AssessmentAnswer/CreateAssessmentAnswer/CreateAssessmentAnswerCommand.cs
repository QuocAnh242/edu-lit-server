using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswer
{
    public class CreateAssessmentAnswerCommand: ICommand<int>
    {
        public int AssessmentQuestionId { get; set; }
        public int AttemptsId { get; set; }

        /// <summary>
        /// A, B, C, D mà student chọn
        /// </summary>
        public string SelectedAnswer { get; set; } = null!;
        public bool IsCorrect { get; set; } = false;
    }
}
