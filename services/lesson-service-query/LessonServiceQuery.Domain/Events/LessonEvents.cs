namespace LessonServiceQuery.Domain.Events;

public class LessonCreatedEvent
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LessonUpdatedEvent
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class LessonDeletedEvent
{
    public Guid LessonId { get; set; }
    public DateTime DeletedAt { get; set; }
}
