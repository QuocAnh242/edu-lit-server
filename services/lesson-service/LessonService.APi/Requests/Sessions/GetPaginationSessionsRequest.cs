namespace LessonService.Api.Requests.Sessions;

public class GetPaginationSessionsRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? CourseId { get; set; }
}


