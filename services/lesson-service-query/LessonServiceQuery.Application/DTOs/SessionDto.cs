namespace LessonServiceQuery.Application.DTOs;
public class SessionDto
{
    public Guid SessionId { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Position { get; set; }
    public int? DurationMinutes { get; set; }
    public Guid? LessonId { get; set; }
    public List<string> Objectives { get; set; } = new();
    public List<string> Materials { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}