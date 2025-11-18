using FluentValidation;
using LessonService.Application.Features.Syllabus.UpdateSyllabus;

namespace LessonService.Application.Features.Syllabus.Update;

public class UpdateSyllabusCommandValidator : AbstractValidator<UpdateSyllabusCommand>
{
    public UpdateSyllabusCommandValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Title), () =>
        {
            RuleFor(x => x.Title)
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters");
        });

        When(x => !string.IsNullOrWhiteSpace(x.AcademicYear), () =>
        {
            RuleFor(x => x.AcademicYear)
                .MaximumLength(50)
                .WithMessage("Academic Year must not exceed 50 characters");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters");
        });
    }
}

