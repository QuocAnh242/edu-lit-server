namespace LessonService.Application.Features.LessonContexts.GetPaginationLessonContexts;

public class GetLessonContextsResponse
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid? ParentLessonId { get; set; }
    public string LessonTitle { get; set; } = null!;
    public string? LessonContent { get; set; }
    public int Position { get; set; }
    public int? Level { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

