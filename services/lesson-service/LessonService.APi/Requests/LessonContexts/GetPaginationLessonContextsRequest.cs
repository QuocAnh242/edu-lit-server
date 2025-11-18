namespace LessonService.Api.Requests.LessonContexts;

public class GetPaginationLessonContextsRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? ParentLessonId { get; set; }
}

