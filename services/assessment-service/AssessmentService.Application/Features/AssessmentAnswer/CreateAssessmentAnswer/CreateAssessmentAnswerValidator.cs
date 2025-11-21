using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswer
{
    public class CreateAssessmentAnswerValidator : AbstractValidator<CreateAssessmentAnswerCommand>
    {
        public CreateAssessmentAnswerValidator()
        {
            RuleFor(x => x.AssessmentQuestionId)
                .GreaterThan(0).WithMessage("AssessmentQuestionId must be greater than 0.");
            RuleFor(x => x.AttemptsId)
                .GreaterThan(0).WithMessage("AttemptsId must be greater than 0.");
            RuleFor(x => x.SelectedOptionId)
                .NotEmpty().WithMessage("SelectedOptionId is required.");
        }
    }
}
