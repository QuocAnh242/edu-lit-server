using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.LessonContexts.CreateBulkLessonContexts;

public record CreateBulkLessonContextsCommand : ICommand<List<Guid>>
{
    public Guid SessionId { get; set; }
    public List<LessonContextItem> LessonContexts { get; set; } = new();
}

public record LessonContextItem
{
    public Guid? ParentLessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public string? LessonContent { get; set; }
    public int Position { get; set; }
    public int Level { get; set; }
}

