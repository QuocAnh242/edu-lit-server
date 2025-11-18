namespace LessonServiceQuery.Domain.Events;

public class ActivityCreatedEvent
{
    public Guid ActivityId { get; set; }
    public Guid LessonContextId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int EstimatedTimeMinutes { get; set; }
    public int OrderIndex { get; set; }
    public List<string> Materials { get; set; } = new();
    public List<string> Objectives { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ActivityUpdatedEvent
{
    public Guid ActivityId { get; set; }
    public Guid LessonContextId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int EstimatedTimeMinutes { get; set; }
    public int OrderIndex { get; set; }
    public List<string> Materials { get; set; } = new();
    public List<string> Objectives { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}

public class ActivityDeletedEvent
{
    public Guid ActivityId { get; set; }
    public Guid LessonContextId { get; set; }
    public DateTime DeletedAt { get; set; }
}
