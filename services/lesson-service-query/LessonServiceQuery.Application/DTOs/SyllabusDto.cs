namespace LessonServiceQuery.Application.DTOs;
public class SyllabusDto
{
    public Guid SyllabusId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public List<CourseDto> Courses { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}