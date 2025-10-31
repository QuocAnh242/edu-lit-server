using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.LessonContexts.CreateLessonContext;

public class CreateLessonContextCommand : ICommand<Guid>
{
    public Guid SessionId { get; set; }
    public Guid? ParentLessonId { get; set; }
    public string LessonTitle { get; set; } = null!;
    public string? LessonContent { get; set; }
    public int Position { get; set; }
    public int? Level { get; set; }
}