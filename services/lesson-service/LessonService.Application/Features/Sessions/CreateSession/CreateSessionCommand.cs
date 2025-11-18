using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Sessions.CreateSession;

public class CreateSessionCommand : ICommand<Guid>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Position { get; set; }
    public int? DurationMinutes { get; set; }
}