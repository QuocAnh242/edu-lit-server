using System.Text.Json;
using LessonService.Application.Services;
using LessonService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace LessonService.Infrastructure.BackgroundServices;

public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private bool _tableCheckPassed = false;

    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service started");

        // Wait a bit for migrations to complete
        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
                
                if (!_tableCheckPassed)
                {
                    _tableCheckPassed = true;
                    _logger.LogInformation("✅ outbox_messages table verified and working");
                }
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == "42P01")
            {
                // Table doesn't exist - log once and wait longer
                if (_tableCheckPassed == false)
                {
                    _logger.LogWarning(
                        "⚠️  outbox_messages table does not exist yet. " +
                        "Waiting for migrations to complete. Will retry every {Interval} seconds.",
                        _interval.TotalSeconds);
                    _tableCheckPassed = false; // Keep it false to avoid repeated logs
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Outbox Processor Service stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMQPublisher>();

        var messages = await unitOfWork.OutboxRepository.GetUnprocessedMessagesAsync(batchSize: 10);
        
        var messageCount = messages.Count();
        if (messageCount > 0)
        {
            _logger.LogInformation("🔄 Processing {Count} outbox messages...", messageCount);
        }

        foreach (var message in messages)
        {
            try
            {
                _logger.LogInformation("📦 Processing outbox message:");
                _logger.LogInformation("  Message ID: {MessageId}", message.Id);
                _logger.LogInformation("  Event Type: {EventType}", message.Type);
                _logger.LogInformation("  Exchange: {Exchange}", message.Exchange);
                _logger.LogInformation("  Created At: {CreatedAt}", message.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                _logger.LogInformation("  Retry Count: {RetryCount}/3", message.RetryCount);
                
                // Deserialize payload to get the event data
                var payload = JsonSerializer.Deserialize<object>(message.Payload);

                if (payload == null)
                {
                    _logger.LogWarning("⚠️  Outbox message {MessageId} has null payload, marking as failed", message.Id);
                    await unitOfWork.OutboxRepository.MarkAsFailedAsync(message.Id, "Null payload");
                    continue;
                }
                
                // Log the payload that will be sent
                _logger.LogInformation("  📄 Event Payload (from Outbox):");
                _logger.LogInformation("{Payload}", message.Payload);

                // Publish to RabbitMQ using the exchange and routing key stored in the message
                var routingKey = string.IsNullOrEmpty(message.RoutingKey) ? "#" : message.RoutingKey;
                await publisher.PublishAsync(message.Exchange, routingKey, payload);

                // Mark as processed
                await unitOfWork.OutboxRepository.MarkAsProcessedAsync(message.Id);

                _logger.LogInformation("✅ Successfully processed outbox message {MessageId}", message.Id);
                _logger.LogInformation("  Event: {EventType} published to {Exchange}", message.Type, message.Exchange);
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                _logger.LogWarning("⚠️  RabbitMQ unavailable - message {MessageId} will be retried", message.Id);
                _logger.LogDebug(ex, "RabbitMQ broker unreachable details");
                
                await unitOfWork.OutboxRepository.MarkAsFailedAsync(message.Id, "RabbitMQ broker unreachable");
                
                // Stop processing remaining messages if RabbitMQ is down
                _logger.LogWarning("  Stopping outbox processing until RabbitMQ is available");
                break;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                _logger.LogWarning("⚠️  Cannot connect to RabbitMQ - message {MessageId} will be retried", message.Id);
                _logger.LogDebug(ex, "Socket connection failed");
                
                await unitOfWork.OutboxRepository.MarkAsFailedAsync(message.Id, "Cannot connect to RabbitMQ");
                
                // Stop processing remaining messages if RabbitMQ is down
                _logger.LogWarning("  Stopping outbox processing until RabbitMQ is available");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to process outbox message {MessageId}", message.Id);
                _logger.LogError("  Event Type: {EventType}", message.Type);
                _logger.LogError("  Exchange: {Exchange}", message.Exchange);
                _logger.LogError("  Error: {ErrorMessage}", ex.Message);
                
                await unitOfWork.OutboxRepository.MarkAsFailedAsync(message.Id, ex.Message);
                
                _logger.LogWarning("  Marked as failed. Retry count: {RetryCount}/3", message.RetryCount + 1);
            }
        }
        
        if (messageCount > 0)
        {
            _logger.LogInformation("✅ Completed processing {Count} outbox messages", messageCount);
        }
    }
}

