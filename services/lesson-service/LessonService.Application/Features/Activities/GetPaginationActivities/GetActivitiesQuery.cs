using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;

namespace LessonService.Application.Features.Activities.GetPaginationActivities;

public class GetActivitiesQuery : IQuery<PagedResult<GetActivitiesResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? SessionId { get; set; }
    public string? ActivityType { get; set; }
}