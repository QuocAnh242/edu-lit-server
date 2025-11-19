using System.Text;
using System.Text.Json;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LessonServiceQuery.Infrastructure.Services.Consumers;

public class UniversalEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UniversalEventConsumer> _logger;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private IConnection? _connection;
    private IChannel? _channel;

    private readonly Dictionary<string, (string Exchange, string[] Queues, string[] RoutingKeys)> _entityConfigurations;

    public UniversalEventConsumer(
        IServiceProvider serviceProvider,
        ILogger<UniversalEventConsumer> logger,
        IOptions<RabbitMqSettings> rabbitMqSettings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _rabbitMqSettings = rabbitMqSettings.Value;

        // Configure all entities
        _entityConfigurations = new Dictionary<string, (string, string[], string[])>
        {
            ["syllabus"] = (
                _rabbitMqSettings.SyllabusExchange,
                new[] { 
                    _rabbitMqSettings.SyllabusCreatedQueue, 
                    _rabbitMqSettings.SyllabusUpdatedQueue, 
                    _rabbitMqSettings.SyllabusDeletedQueue 
                },
                new[] { "syllabus.created", "syllabus.updated", "syllabus.deleted" }
            ),
            ["course"] = (
                _rabbitMqSettings.CourseraExchange,
                new[] { 
                    _rabbitMqSettings.CourseraCreatedQueue, 
                    _rabbitMqSettings.CourseraUpdatedQueue, 
                    _rabbitMqSettings.CourseraDeletedQueue 
                },
                new[] { "course.created", "course.updated", "course.deleted" }
            ),
            ["session"] = (
                _rabbitMqSettings.SessionExchange ?? "session-events",
                new[] { 
                    _rabbitMqSettings.SessionCreatedQueue ?? "session-created-query", 
                    _rabbitMqSettings.SessionUpdatedQueue ?? "session-updated-query", 
                    _rabbitMqSettings.SessionDeletedQueue ?? "session-deleted-query" 
                },
                new[] { "session.created", "session.updated", "session.deleted" }
            ),
            ["lesson"] = (
                _rabbitMqSettings.LessonExchange,
                new[] { 
                    _rabbitMqSettings.LessonCreatedQueue, 
                    _rabbitMqSettings.LessonUpdatedQueue, 
                    _rabbitMqSettings.LessonDeletedQueue 
                },
                new[] { "lesson.created", "lesson.updated", "lesson.deleted" }
            ),
            ["lessoncontext"] = (
                _rabbitMqSettings.LessonContextExchange,
                new[] { 
                    _rabbitMqSettings.LessonContextCreatedQueue, 
                    _rabbitMqSettings.LessonContextUpdatedQueue, 
                    _rabbitMqSettings.LessonContextDeletedQueue 
                },
                new[] { "lessoncontext.created", "lessoncontext.updated", "lessoncontext.deleted" }
            ),
            ["activity"] = (
                _rabbitMqSettings.ActivityExchange,
                new[] { 
                    _rabbitMqSettings.ActivityCreatedQueue, 
                    _rabbitMqSettings.ActivityUpdatedQueue, 
                    _rabbitMqSettings.ActivityDeletedQueue 
                },
                new[] { "activity.created", "activity.updated", "activity.deleted" }
            )
        };
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Universal Event Consumer...");
        await InitializeRabbitMqAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
    {
        const int maxRetries = 10;
        const int initialDelayMs = 1000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation("Attempting to connect to RabbitMQ (attempt {Attempt}/{MaxRetries})...", attempt, maxRetries);

                var factory = new ConnectionFactory
                {
                    HostName = _rabbitMqSettings.HostName,
                    Port = _rabbitMqSettings.Port,
                    UserName = _rabbitMqSettings.UserName,
                    Password = _rabbitMqSettings.Password,
                    VirtualHost = _rabbitMqSettings.VirtualHost
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                // Setup exchanges, queues and bindings for all entities
                foreach (var (entityType, config) in _entityConfigurations)
                {
                    _logger.LogInformation("Setting up RabbitMQ for {EntityType}", entityType);

                    // Declare exchange
                    await _channel.ExchangeDeclareAsync(
                        exchange: config.Exchange,
                        type: ExchangeType.Topic,
                        durable: true,
                        autoDelete: false,
                        cancellationToken: cancellationToken);

                    // Declare and bind queues
                    for (int i = 0; i < config.Queues.Length; i++)
                    {
                        await _channel.QueueDeclareAsync(
                            queue: config.Queues[i],
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null,
                            cancellationToken: cancellationToken);

                        await _channel.QueueBindAsync(
                            queue: config.Queues[i],
                            exchange: config.Exchange,
                            routingKey: config.RoutingKeys[i],
                            cancellationToken: cancellationToken);
                    }
                }

                _logger.LogInformation("RabbitMQ connection established for Universal Event Consumer");
                return; // Success - exit retry loop
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ after {MaxRetries} attempts", maxRetries);
                    throw;
                }

                int delayMs = initialDelayMs * (int)Math.Pow(2, attempt - 1); // Exponential backoff
                _logger.LogWarning(ex, "Failed to connect to RabbitMQ (attempt {Attempt}/{MaxRetries}). Retrying in {DelayMs}ms...", 
                    attempt, maxRetries, delayMs);
                
                await Task.Delay(delayMs, cancellationToken);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        // Start consuming from all queues
        foreach (var (entityType, config) in _entityConfigurations)
        {
            for (int i = 0; i < config.Queues.Length; i++)
            {
                string eventType = config.RoutingKeys[i].Split('.')[1]; // created, updated, deleted
                await ConsumeQueue(config.Queues[i], entityType, eventType, stoppingToken);
            }
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ConsumeQueue(string queueName, string entityType, string eventType, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var startTime = DateTime.UtcNow;
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                _logger.LogInformation("========================================");
                _logger.LogInformation("📥 RECEIVED MESSAGE from queue: {QueueName}", queueName);
                _logger.LogInformation("Entity Type: {EntityType}", entityType);
                _logger.LogInformation("Event Type: {EventType}", eventType);
                _logger.LogInformation("Delivery Tag: {DeliveryTag}", eventArgs.DeliveryTag);
                _logger.LogInformation("Message Content: {Message}", message);
                _logger.LogInformation("========================================");

                await HandleEvent(entityType, eventType, message);
                await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false);

                var duration = DateTime.UtcNow - startTime;
                _logger.LogInformation("✅ SUCCESS: Processed {EventType} event for {EntityType} in {Duration}ms", 
                    eventType, entityType, duration.TotalMilliseconds);
                _logger.LogInformation("========================================");
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                _logger.LogError("========================================");
                _logger.LogError("❌ ERROR: Failed to process {EventType} event for {EntityType} after {Duration}ms", 
                    eventType, entityType, duration.TotalMilliseconds);
                _logger.LogError(ex, "Exception details");
                _logger.LogError("Message that caused error: {Message}", message);
                _logger.LogError("========================================");
                await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, true);
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation("🚀 Started consuming from queue: {QueueName} for {EntityType}.{EventType}", 
            queueName, entityType, eventType);
    }

    private async Task HandleEvent(string entityType, string eventType, string message)
    {
        _logger.LogInformation("🔄 Routing event to handler: {EntityType}.{EventType}", entityType, eventType);
        
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<UniversalEventHandler>();

        var jsonDoc = JsonDocument.Parse(message);
        var root = jsonDoc.RootElement;

        _logger.LogInformation("📋 Parsed JSON document successfully");

        switch (eventType.ToLower())
        {
            case "created":
                _logger.LogInformation("➡️ Calling HandleCreatedEvent for {EntityType}", entityType);
                await handler.HandleCreatedEvent(entityType, root);
                break;
            case "updated":
                _logger.LogInformation("➡️ Calling HandleUpdatedEvent for {EntityType}", entityType);
                await handler.HandleUpdatedEvent(entityType, root);
                break;
            case "deleted":
                if (root.TryGetProperty("Id", out var idElement) || 
                    root.TryGetProperty("id", out idElement))
                {
                    var id = idElement.GetGuid();
                    _logger.LogInformation("➡️ Calling HandleDeletedEvent for {EntityType} with ID: {Id}", entityType, id);
                    await handler.HandleDeletedEvent(entityType, id);
                }
                else
                {
                    _logger.LogWarning("⚠️ Delete event for {EntityType} missing Id property", entityType);
                }
                break;
            default:
                _logger.LogWarning("⚠️ Unknown event type: {EventType}", eventType);
                break;
        }
        
        _logger.LogInformation("✓ Handler completed for {EntityType}.{EventType}", entityType, eventType);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Universal Event Consumer...");
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}

