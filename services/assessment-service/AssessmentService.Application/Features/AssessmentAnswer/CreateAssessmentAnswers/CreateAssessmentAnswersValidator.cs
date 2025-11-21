using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswers
{
    public class CreateAssessmentAnswersValidator : AbstractValidator<CreateAssessmentAnswersCommand>
    {
        public CreateAssessmentAnswersValidator()
        {
            RuleFor(x => x.AttemptsId)
                .GreaterThan(0)
                .WithMessage("AttemptsId phải lớn hơn 0");

            RuleFor(x => x.SelectedOptionIds)
                .NotEmpty()
                .WithMessage("SelectedOptionIds không được để trống")
                .Must(ids => ids != null && ids.Count > 0)
                .WithMessage("Phải có ít nhất 1 SelectedOptionId");

            RuleForEach(x => x.SelectedOptionIds)
                .NotEmpty()
                .WithMessage("SelectedOptionId không được để trống");
        }
    }
}

