namespace LessonServiceQuery.Application.DTOs;
public class CourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationWeeks { get; set; }
    public List<string> Objectives { get; set; } = new();
    public List<SessionDto> Sessions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}