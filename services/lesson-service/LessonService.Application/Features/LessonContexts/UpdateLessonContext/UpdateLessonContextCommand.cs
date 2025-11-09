using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.LessonContexts.UpdateLessonContext;

public class UpdateLessonContextCommand : ICommand
{
    public Guid Id { get; set; }
    public string? LessonTitle { get; set; }
    public string? LessonContent { get; set; }
    public int? Position { get; set; }
    public int? Level { get; set; }
}