using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Courses.UpdateCourse;

public class UpdateCourseCommand : ICommand
{
    public Guid Id { get; set; }
    public string? CourseCode { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}