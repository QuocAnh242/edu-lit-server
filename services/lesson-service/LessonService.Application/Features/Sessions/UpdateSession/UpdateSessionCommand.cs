using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Sessions.UpdateSession;

public class UpdateSessionCommand : ICommand
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Position { get; set; }
    public int? DurationMinutes { get; set; }
}