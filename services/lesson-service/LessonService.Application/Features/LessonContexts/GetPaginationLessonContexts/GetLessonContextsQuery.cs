using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;

namespace LessonService.Application.Features.LessonContexts.GetPaginationLessonContexts;

public class GetLessonContextsQuery : IQuery<PagedResult<GetLessonContextsResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? ParentLessonId { get; set; }
}