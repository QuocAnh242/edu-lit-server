namespace LessonService.Application.Services;

public interface IRabbitMQPublisher
{
    Task PublishAsync(string exchange, string routingKey, object message);
}

