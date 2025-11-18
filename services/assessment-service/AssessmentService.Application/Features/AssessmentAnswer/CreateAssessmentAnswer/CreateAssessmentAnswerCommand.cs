using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswer
{
    public class CreateAssessmentAnswerCommand: ICommand<int>
    {
        public int AssessmentQuestionId { get; set; }
        public int AttemptsId { get; set; }

        /// <summary>
        /// Reference to QuestionOption in Question Service
        /// </summary>
        public Guid SelectedOptionId { get; set; }
    }
}
