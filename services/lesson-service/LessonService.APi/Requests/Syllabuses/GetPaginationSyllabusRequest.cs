using LessonService.Domain.Enums;

namespace LessonService.Api.Requests;

public class GetPaginationSyllabusRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Semester? Semester { get; set; }
    public bool? IsActive { get; set; }
}