namespace LessonService.Application.Features.Sessions.GetSessionById;

public class GetSessionByIdResponse
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

