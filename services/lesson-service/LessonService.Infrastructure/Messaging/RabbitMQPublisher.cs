using System.Text;
using System.Text.Json;
using LessonService.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
namespace LessonService.Infrastructure.Messaging;
public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _isInitialized;
    private readonly HashSet<string> _declaredExchanges = new();
    public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _logger.LogInformation("RabbitMQ Publisher initialized (connection will be established on first publish)");
    }
    private async Task EnsureConnectionAsync()
    {
        if (_isInitialized && _connection != null && _channel != null)
            return;
        await _connectionLock.WaitAsync();
        try
        {
            if (_isInitialized && _connection != null && _channel != null)
                return;
            _logger.LogInformation("Establishing RabbitMQ connection...");
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? _configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"] ?? _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                SocketReadTimeout = TimeSpan.FromSeconds(30),
                SocketWriteTimeout = TimeSpan.FromSeconds(30)
            };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            _isInitialized = true;
            _logger.LogInformation("RabbitMQ connection established successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }
    private async Task EnsureExchangeDeclaredAsync(string exchangeName)
    {
        if (_declaredExchanges.Contains(exchangeName))
            return;
        await _channel!.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Topic, // Changed from Fanout to Topic to match Consumer
            durable: true,
            autoDelete: false
        );
        _declaredExchanges.Add(exchangeName);
        _logger.LogInformation("Declared exchange: {Exchange} (type: topic)", exchangeName);
    }
    public async Task PublishAsync(string exchange, string routingKey, object message)
    {
        var startTime = DateTimeOffset.UtcNow;
        
        try
        {
            _logger.LogInformation("📤 Publishing message to RabbitMQ...");
            _logger.LogInformation("  Exchange: {Exchange}", exchange);
            _logger.LogInformation("  Routing Key: {RoutingKey}", string.IsNullOrEmpty(routingKey) ? "(empty - fanout)" : routingKey);
            
            await EnsureConnectionAsync();
            await EnsureExchangeDeclaredAsync(exchange);
            
            // Serialize message with indented format for logging
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(message, options);
            var compactJson = JsonSerializer.Serialize(message, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            var body = Encoding.UTF8.GetBytes(compactJson);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Timestamp = new AmqpTimestamp(timestamp),
                MessageId = Guid.NewGuid().ToString(),
                AppId = "lesson-service"
            };
            
            // Log message details
            _logger.LogInformation("  Message ID: {MessageId}", properties.MessageId);
            _logger.LogInformation("  Message Size: {Size} bytes", body.Length);
            _logger.LogInformation("  Timestamp: {Timestamp} (Unix: {UnixTimestamp})", 
                DateTimeOffset.FromUnixTimeSeconds(timestamp).ToString("yyyy-MM-dd HH:mm:ss"), 
                timestamp);
            _logger.LogInformation("  Message Type: {Type}", message.GetType().Name);
            
            // Log message payload (pretty printed)
            _logger.LogInformation("  📋 Message Payload:\n{Payload}", json);
            
            await _channel!.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
            
            var duration = (DateTimeOffset.UtcNow - startTime).TotalMilliseconds;
            
            _logger.LogInformation("✅ Successfully published message to exchange '{Exchange}'", exchange);
            _logger.LogInformation("  Duration: {Duration}ms", duration);
            _logger.LogInformation("  Connection: {Host}:{Port}", 
                _configuration["RabbitMQ:Host"] ?? "localhost",
                _configuration["RabbitMQ:Port"] ?? "5672");
        }
        catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
        {
            var duration = (DateTimeOffset.UtcNow - startTime).TotalMilliseconds;
            
            _logger.LogWarning(ex, "⚠️  RabbitMQ broker is unreachable");
            _logger.LogWarning("  Exchange: {Exchange}", exchange);
            _logger.LogWarning("  Host: {Host}:{Port}", 
                _configuration["RabbitMQ:Host"] ?? "localhost",
                _configuration["RabbitMQ:Port"] ?? "5672");
            _logger.LogWarning("  Make sure RabbitMQ is running:");
            _logger.LogWarning("    docker run -d -p 5672:5672 -p 15672:15672 --name rabbitmq rabbitmq:3-management");
            _logger.LogWarning("  Message will remain in outbox and be retried later");
            
            throw;
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            var duration = (DateTimeOffset.UtcNow - startTime).TotalMilliseconds;
            
            _logger.LogWarning(ex, "⚠️  Cannot connect to RabbitMQ - Connection refused");
            _logger.LogWarning("  Exchange: {Exchange}", exchange);
            _logger.LogWarning("  Attempting to connect to: {Host}:{Port}", 
                _configuration["RabbitMQ:Host"] ?? "localhost",
                _configuration["RabbitMQ:Port"] ?? "5672");
            _logger.LogWarning("  Message will remain in outbox and be retried later");
            
            throw;
        }
        catch (Exception ex)
        {
            var duration = (DateTimeOffset.UtcNow - startTime).TotalMilliseconds;
            
            _logger.LogError(ex, "❌ Failed to publish message to RabbitMQ");
            _logger.LogError("  Exchange: {Exchange}", exchange);
            _logger.LogError("  Routing Key: {RoutingKey}", routingKey);
            _logger.LogError("  Duration before failure: {Duration}ms", duration);
            _logger.LogError("  Error Type: {ErrorType}", ex.GetType().Name);
            _logger.LogError("  Error Message: {ErrorMessage}", ex.Message);
            
            // Re-throw to allow caller to handle
            throw;
        }
    }
    public void Dispose()
    {
        try
        {
            if (_channel != null)
            {
                _channel.CloseAsync().GetAwaiter().GetResult();
                _channel.Dispose();
            }
            if (_connection != null)
            {
                _connection.CloseAsync().GetAwaiter().GetResult();
                _connection.Dispose();
            }
            _connectionLock.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
    }
}
