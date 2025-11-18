namespace LessonService.Api.Requests.LessonContexts;

public class CreateLessonContextRequest
{
    public Guid SessionId { get; set; }
    public Guid? ParentLessonId { get; set; }
    public string LessonTitle { get; set; } = null!;
    public string? LessonContent { get; set; }
    public int Position { get; set; }
    public int? Level { get; set; }
}