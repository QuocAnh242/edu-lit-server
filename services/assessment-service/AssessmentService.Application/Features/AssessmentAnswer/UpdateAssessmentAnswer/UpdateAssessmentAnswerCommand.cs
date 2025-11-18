using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer
{
    public class UpdateAssessmentAnswerCommand : ICommand<bool>
    {
        public int AnswerId { get; set; }

        public int AssessmentQuestionId { get; set; }

        public int AttemptsId { get; set; }

        /// <summary>
        /// Reference to QuestionOption in Question Service
        /// </summary>
        public Guid SelectedOptionId { get; set; }
    }
}
