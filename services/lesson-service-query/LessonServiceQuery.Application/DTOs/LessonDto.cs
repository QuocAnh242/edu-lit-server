namespace LessonServiceQuery.Application.DTOs;
public class LessonDto
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public Guid TeacherId { get; set; }
    public List<LessonContextDto> LessonContexts { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}