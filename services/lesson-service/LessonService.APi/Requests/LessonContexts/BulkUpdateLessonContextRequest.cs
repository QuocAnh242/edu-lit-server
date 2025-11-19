namespace LessonService.Api.Requests.LessonContexts;

public class BulkUpdateLessonContextRequest
{
    public List<LessonContextUpdateItem> Items { get; set; } = new();
}

public class LessonContextUpdateItem
{
    public Guid Id { get; set; }
    public string? LessonTitle { get; set; }
    public string? LessonContent { get; set; }
    public int? Position { get; set; }
    public int? Level { get; set; }
}

