namespace LessonService.Application.Features.Sessions.GetPaginationSessions;

public class GetSessionsResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Position { get; set; }
    public int? DurationMinutes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

