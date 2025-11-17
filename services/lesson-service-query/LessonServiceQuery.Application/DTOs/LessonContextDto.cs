namespace LessonServiceQuery.Application.DTOs;
public class LessonContextDto
{
    public Guid LessonContextId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public List<ActivityDto> Activities { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}