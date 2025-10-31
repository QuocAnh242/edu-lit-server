using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;

namespace LessonService.Application.Features.Courses.GetPaginationCourses;

public class GetCoursesQuery : IQuery<PagedResult<GetCoursesResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? SyllabusId { get; set; }
}