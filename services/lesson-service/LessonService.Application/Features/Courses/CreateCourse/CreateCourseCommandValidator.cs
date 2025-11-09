using FluentValidation;

namespace LessonService.Application.Features.Courses.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.SyllabusId)
            .NotEmpty().WithMessage("SyllabusId is required");
        
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("CourseCode is required")
            .MaximumLength(50).WithMessage("CourseCode must not exceed 50 characters");
        
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}

