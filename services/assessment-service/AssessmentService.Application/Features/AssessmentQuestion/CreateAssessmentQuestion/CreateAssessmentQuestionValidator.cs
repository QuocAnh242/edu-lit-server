using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion
{
    public class CreateAssessmentQuestionValidator : AbstractValidator<CreateAssessmentQuestionCommand>
    {
        public CreateAssessmentQuestionValidator()
        {
            RuleFor(x => x.AssessmentId)
                .GreaterThan(0)
                .WithMessage("AssessmentId phải lớn hơn 0");

            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .WithMessage("QuestionId không được để trống");
        }
    }
}
