using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Courses.DeleteCourse;

public class DeleteCourseCommand : ICommand
{
    public Guid Id { get; }
    
    public DeleteCourseCommand(Guid id)
    {
        Id = id;
    }
}