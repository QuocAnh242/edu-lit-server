namespace LessonServiceQuery.Application.DTOs;

public class LessonContextDto
{
    public Guid LessonContextId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Position { get; set; }
    public int Level { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}