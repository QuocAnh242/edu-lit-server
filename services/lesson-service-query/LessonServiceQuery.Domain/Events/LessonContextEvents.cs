namespace LessonServiceQuery.Domain.Events;

public class LessonContextCreatedEvent
{
    public Guid LessonContextId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LessonContextUpdatedEvent
{
    public Guid LessonContextId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class LessonContextDeletedEvent
{
    public Guid LessonContextId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime DeletedAt { get; set; }
}
