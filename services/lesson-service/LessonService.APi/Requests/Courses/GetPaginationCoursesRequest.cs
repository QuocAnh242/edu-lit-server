namespace LessonService.Api.Requests.Courses;

public class GetPaginationCoursesRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? SyllabusId { get; set; }
}


