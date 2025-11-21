namespace LessonService.Api.Requests.LessonContexts;

public class CreateBulkLessonContextRequest
{
    public Guid SessionId { get; set; }
    public List<LessonContextItemRequest> LessonContexts { get; set; } = new();
}

public class LessonContextItemRequest
{
    public string LessonTitle { get; set; } = null!;
    public string? LessonContent { get; set; }
    public int Position { get; set; }
    public int Level { get; set; }
}

