using FluentValidation;

namespace AssessmentService.Application.Features.Assessment.UpdateAssessment
{
    public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentCommand>
    {
        public UpdateAssessmentValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required.")
                .MaximumLength(100).WithMessage("CourseId cannot exceed 100 characters.");

            RuleFor(x => x.CreatorId)
                .NotEmpty().WithMessage("CreatorId is required.")
                .MaximumLength(100).WithMessage("CreatorId cannot exceed 100 characters.");

            // Validate Title
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MinimumLength(3)
                .WithMessage("Title must be at least 3 characters")
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters");

            // Validate Description (optional)
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // Validate TotalQuestions
            RuleFor(x => x.TotalQuestions)
                .GreaterThan(0)
                .WithMessage("TotalQuestions must be greater than 0")
                .LessThanOrEqualTo(100)
                .WithMessage("TotalQuestions must not exceed 100");

            // Validate DurationMinutes
            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0)
                .WithMessage("DurationMinutes must be greater than 0")
                .LessThanOrEqualTo(300)
                .WithMessage("DurationMinutes must not exceed 300 minutes (5 hours)");
        }
    }
}
