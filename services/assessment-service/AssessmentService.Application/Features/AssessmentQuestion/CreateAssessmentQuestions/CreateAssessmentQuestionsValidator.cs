using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestions
{
    public class CreateAssessmentQuestionsValidator : AbstractValidator<CreateAssessmentQuestionsCommand>
    {
        public CreateAssessmentQuestionsValidator()
        {
            RuleFor(x => x.AssessmentId)
                .GreaterThan(0)
                .WithMessage("AssessmentId phải lớn hơn 0");

            RuleFor(x => x.QuestionIds)
                .NotEmpty()
                .WithMessage("QuestionIds không được để trống")
                .Must(ids => ids != null && ids.Count > 0)
                .WithMessage("Phải có ít nhất 1 QuestionId");

            RuleForEach(x => x.QuestionIds)
                .NotEmpty()
                .WithMessage("QuestionId không được để trống");
        }
    }
}

