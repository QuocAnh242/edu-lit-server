namespace LessonService.Api.Requests.Sessions;

public class UpdateSessionRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Position { get; set; }
    public int? DurationMinutes { get; set; }
}

