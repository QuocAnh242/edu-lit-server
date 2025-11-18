using FluentValidation;

namespace AssessmentService.Application.Features.AssignmentAttempt.CreateAssignmentAttempt
{
    public class CreateAssignmentAttemptValidator : AbstractValidator<CreateAssignmentAttemptCommand>
    {
        public CreateAssignmentAttemptValidator()
        {
            RuleFor(x => x.AssessmentId)
                .GreaterThan(0).WithMessage("AssessmentId must be greater than 0.");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
