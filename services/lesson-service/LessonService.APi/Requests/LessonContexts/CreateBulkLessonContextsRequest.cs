namespace LessonService.Api.Requests.LessonContexts;

public class CreateBulkLessonContextsRequest
{
    public Guid SessionId { get; set; }
    public List<LessonContextItemRequest> LessonContexts { get; set; } = new();
}

public class LessonContextItemRequest
{
    public Guid? ParentLessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public string? LessonContent { get; set; }
    public int Position { get; set; }
    public int Level { get; set; }
}

