using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;

namespace LessonService.Application.Features.Sessions.GetPaginationSessions;

public class GetSessionsQuery : IQuery<PagedResult<GetSessionsResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? CourseId { get; set; }
}