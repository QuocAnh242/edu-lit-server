using FluentValidation;

namespace LessonService.Application.Features.LessonContexts.CreateBulkLessonContexts;

public class CreateBulkLessonContextsCommandValidator : AbstractValidator<CreateBulkLessonContextsCommand>
{
    public CreateBulkLessonContextsCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("SessionId is required");

        RuleFor(x => x.LessonContexts)
            .NotEmpty().WithMessage("At least one LessonContext is required")
            .Must(x => x.Count <= 100).WithMessage("Maximum 100 LessonContexts can be created at once");

        RuleForEach(x => x.LessonContexts).ChildRules(item =>
        {
            item.RuleFor(x => x.LessonTitle)
                .NotEmpty().WithMessage("LessonTitle is required")
                .MaximumLength(500).WithMessage("LessonTitle must not exceed 500 characters");

            item.RuleFor(x => x.Position)
                .GreaterThanOrEqualTo(0).WithMessage("Position must be >= 0");

            item.RuleFor(x => x.Level)
                .GreaterThanOrEqualTo(0).WithMessage("Level must be >= 0")
                .LessThanOrEqualTo(10).WithMessage("Level must be <= 10");
        });
    }
}

