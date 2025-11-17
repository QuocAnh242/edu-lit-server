using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer
{
    public class UpdateAssessmentAnswerValiator : AbstractValidator<UpdateAssessmentAnswerCommand>
    {
        public UpdateAssessmentAnswerValiator()
        {
            RuleFor(x => x.AssessmentQuestionId)
                .GreaterThan(0).WithMessage("AssessmentQuestionId must be greater than 0.");
            RuleFor(x => x.AttemptsId)
                .GreaterThan(0).WithMessage("AttemptsId must be greater than 0.");
            RuleFor(x => x.SelectedAnswer)
                .NotEmpty().WithMessage("SelectedAnswer is required.")
                .Must(answer => new[] { "A", "B", "C", "D" }.Contains(answer))
                .WithMessage("SelectedAnswer must be one of the following values: A, B, C, D.");
            RuleFor(x => x.IsCorrect)
                .NotNull().WithMessage("IsCorrect is required.");
        }
    }
}
