namespace LessonServiceQuery.Domain.Events;
public class SessionCreatedEvent
{
    public Guid SessionId { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public Guid? LessonId { get; set; }
    public List<string> Objectives { get; set; } = new();
    public List<string> Materials { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
public class SessionUpdatedEvent
{
    public Guid SessionId { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public Guid? LessonId { get; set; }
    public List<string> Objectives { get; set; } = new();
    public List<string> Materials { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
public class SessionDeletedEvent
{
    public Guid SessionId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime DeletedAt { get; set; }
}
