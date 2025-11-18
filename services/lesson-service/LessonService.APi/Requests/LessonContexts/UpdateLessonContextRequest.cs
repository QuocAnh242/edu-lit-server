namespace LessonService.Api.Requests.LessonContexts;

public class UpdateLessonContextRequest
{
    public string? LessonTitle { get; set; }
    public string? LessonContent { get; set; }
    public int? Position { get; set; }
    public int? Level { get; set; }
}

