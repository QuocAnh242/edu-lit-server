namespace LessonServiceQuery.Domain.Events;
public interface IEvent
{
    string EventType { get; set; }
    DateTime Timestamp { get; set; }
    Guid Id { get; set; }
}



