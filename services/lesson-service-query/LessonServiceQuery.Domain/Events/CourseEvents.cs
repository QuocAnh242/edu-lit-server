namespace LessonServiceQuery.Domain.Events;
public class CourseCreatedEvent
{
    public Guid CourseId { get; set; }
    public Guid SyllabusId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationWeeks { get; set; }
    public List<string> Objectives { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
public class CourseUpdatedEvent
{
    public Guid CourseId { get; set; }
    public Guid SyllabusId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationWeeks { get; set; }
    public List<string> Objectives { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
public class CourseDeletedEvent
{
    public Guid CourseId { get; set; }
    public Guid SyllabusId { get; set; }
    public DateTime DeletedAt { get; set; }
}
