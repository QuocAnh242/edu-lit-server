using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion
{
    public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentQuestionCommand>
    {
        public UpdateAssessmentValidator()
        {
            RuleFor(x => x.AssessmentQuestionId)
                .GreaterThan(0)
                .WithMessage("AssessmentQuestionId phải lớn hơn 0");

            RuleFor(x => x.AssessmentId)
                .GreaterThan(0)
                .WithMessage("AssessmentId phải lớn hơn 0");

            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .WithMessage("QuestionId không được để trống");
        }
    }
}
