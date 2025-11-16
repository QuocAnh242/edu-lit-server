using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion
{
    public class CreateAssessmentQuestionCommand : ICommand<int>
    {
        public int AssessmentId { get; set; }
        public string QuestionId { get; set; } = null!;
        public int OrderNum { get; set; }
        /// <summary>
        /// A, B, C, D
        /// </summary>
        public string CorrectAnswer { get; set; } = null!;
    }
}
