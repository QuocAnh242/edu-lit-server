namespace LessonService.Api.Requests.Sessions;

public class CreateSessionRequest
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Position { get; set; }
    public int? DurationMinutes { get; set; }
}