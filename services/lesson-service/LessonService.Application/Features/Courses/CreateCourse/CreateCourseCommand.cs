using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Courses.CreateCourse;

public class CreateCourseCommand : ICommand<Guid>
{
    public Guid SyllabusId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}