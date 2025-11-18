namespace LessonServiceQuery.Domain.Events;

public abstract class BaseEvent : IEvent
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = string.Empty;
}

public class EntityCreatedEvent : BaseEvent
{
    public string EntityType { get; set; } = string.Empty;
    public object Data { get; set; } = null!;
}

public class EntityUpdatedEvent : BaseEvent
{
    public string EntityType { get; set; } = string.Empty;
    public object Data { get; set; } = null!;
}

public class EntityDeletedEvent : BaseEvent
{
    public string EntityType { get; set; } = string.Empty;
}

