namespace LessonService.Api.Requests.Activities;

public class GetPaginationActivitiesRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? SessionId { get; set; }
    public string? ActivityType { get; set; }
}

