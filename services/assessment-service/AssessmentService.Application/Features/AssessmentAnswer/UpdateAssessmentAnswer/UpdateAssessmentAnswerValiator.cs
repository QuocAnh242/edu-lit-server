using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer
{
    public class UpdateAssessmentAnswerValiator : AbstractValidator<UpdateAssessmentAnswerCommand>
    {
        public UpdateAssessmentAnswerValiator()
        {
            RuleFor(x => x.AnswerId)
                .GreaterThan(0).WithMessage("AnswerId must be greater than 0.");
            RuleFor(x => x.AssessmentQuestionId)
                .GreaterThan(0).WithMessage("AssessmentQuestionId must be greater than 0.");
            RuleFor(x => x.AttemptsId)
                .GreaterThan(0).WithMessage("AttemptsId must be greater than 0.");
            RuleFor(x => x.SelectedOptionId)
                .NotEmpty().WithMessage("SelectedOptionId is required.");
        }
    }
}
