using FluentValidation;

namespace LessonService.Application.Features.LessonContexts.CreateLessonContext;

public class CreateLessonContextCommandValidator : AbstractValidator<CreateLessonContextCommand>
{
    public CreateLessonContextCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("SessionId is required");
        
        RuleFor(x => x.LessonTitle)
            .NotEmpty().WithMessage("LessonTitle is required")
            .MaximumLength(200).WithMessage("LessonTitle must not exceed 200 characters");
        
        RuleFor(x => x.Position)
            .GreaterThanOrEqualTo(0).WithMessage("Position must be greater than or equal to 0");
        
        RuleFor(x => x.Level)
            .GreaterThanOrEqualTo(0).When(x => x.Level.HasValue)
            .WithMessage("Level must be greater than or equal to 0");
    }
}

