using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Domain.Entities.ReadModels;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Messaging
{
    public sealed class RabbitMqUserProjectionService : BackgroundService
    {
        private readonly ILogger<RabbitMqUserProjectionService> _logger;
        private readonly IServiceProvider _sp;
        private readonly ConnectionFactory _factory;
        private IConnection? _conn;
        private IChannel? _channel;

        private const string Exchange = "auth.users";
        private const string QueueName = "auth-service-query.users";

        public RabbitMqUserProjectionService(
            IConfiguration cfg,
            ILogger<RabbitMqUserProjectionService> logger,
            IServiceProvider sp)
        {
            _logger = logger;
            _sp = sp;

            var cs = cfg.GetConnectionString("RabbitMq");
            _factory = !string.IsNullOrWhiteSpace(cs)
                ? new ConnectionFactory { Uri = new Uri(cs) }
                : new ConnectionFactory
                {
                    HostName = cfg["RabbitMq:HostName"] ?? "rabbitmq",
                    Port = int.TryParse(cfg["RabbitMq:Port"], out var p) ? p : 5672,
                    UserName = cfg["RabbitMq:UserName"] ?? "guest",
                    Password = cfg["RabbitMq:Password"] ?? "guest",
                    VirtualHost = cfg["RabbitMq:VirtualHost"] ?? "/"
                };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting RabbitMQ User Projection Service...");
                _logger.LogInformation("Connecting to RabbitMQ: {HostName}:{Port}", _factory.HostName, _factory.Port);

                // Retry connection with delay
                var retryCount = 0;
                while (retryCount < 10)
                {
                    try
                    {
                        _conn = await _factory.CreateConnectionAsync(clientProvidedName: "auth-service-query", cancellationToken: stoppingToken);
                        _logger.LogInformation("Connected to RabbitMQ successfully!");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        _logger.LogWarning(ex, "Failed to connect to RabbitMQ (attempt {Retry}/10). Retrying in 5s...", retryCount);
                        await Task.Delay(5000, stoppingToken);
                    }
                }

                if (_conn == null)
                {
                    _logger.LogError("Could not connect to RabbitMQ after 10 attempts. Service stopped.");
                    return;
                }

                _channel = await _conn.CreateChannelAsync(cancellationToken: stoppingToken);
                _logger.LogInformation("Channel created");

                await _channel.ExchangeDeclareAsync(Exchange, ExchangeType.Topic, durable: true, autoDelete: false, arguments: null, cancellationToken: stoppingToken);
                _logger.LogInformation("Exchange declared: {Exchange}", Exchange);

                await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);
                _logger.LogInformation("Queue declared: {Queue}", QueueName);

                foreach (var rk in new[] { 
                    "auth.user.registration.successful", 
                    "auth.user.created", 
                    "auth.user.updated", 
                    "auth.user.deleted",
                    "auth.user.profile.updated",
                    "auth.user.password.changed",
                    "auth.user.password.reset"
                })
                {
                    await _channel.QueueBindAsync(QueueName, Exchange, rk, arguments: null, cancellationToken: stoppingToken);
                    _logger.LogInformation("Queue bound to routing key: {RoutingKey}", rk);
                }

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (_, ea) =>
                {
                    try
                    {
                        var routingKey = ea.RoutingKey;
                        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                        
                        _logger.LogInformation("Received event: {RoutingKey}", routingKey);
                        _logger.LogDebug("Payload: {Payload}", json);
                        
                        await ProjectAsync(routingKey, json, stoppingToken);
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                        
                        _logger.LogInformation("Event processed successfully: {RoutingKey}", routingKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, " Projection failed for event. Nacking message.");
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                    }
                };

                await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
                _logger.LogInformation("RabbitMQ user projection service is now listening for events...");
                _logger.LogInformation("Service started successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in RabbitMQ projection service");
                throw;
            }
        }

        private async Task ProjectAsync(string type, string payload, CancellationToken ct)
        {
            _logger.LogInformation("Projecting event type: {Type}", type);
            _logger.LogInformation("Raw JSON payload: {Payload}", payload);
            
            // Handle double-serialized JSON (payload might be a JSON string containing JSON)
            var actualPayload = payload;
            try
            {
                // Try to deserialize as a string first (in case of double serialization)
                using var testDoc = JsonDocument.Parse(payload);
                if (testDoc.RootElement.ValueKind == JsonValueKind.String)
                {
                    actualPayload = testDoc.RootElement.GetString() ?? payload;
                    _logger.LogInformation("Unwrapped double-serialized JSON");
                }
            }
            catch
            {
                // If it fails, use the original payload
            }
            
            await using var scope = _sp.CreateAsyncScope();
            var dao = scope.ServiceProvider.GetRequiredService<IUserReadDAO>();

            switch (type)
            {
                case "auth.user.registration.successful":
                case "auth.user.created":
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(actualPayload);
                        var root = doc.RootElement;
                        
                        // Try to extract user data from different possible structures
                        JsonElement userData = root;
                        
                        // Check if data is nested (e.g., { "data": {...} } or { "user": {...} })
                        if (root.TryGetProperty("data", out var dataElement))
                            userData = dataElement;
                        else if (root.TryGetProperty("user", out var userElement))
                            userData = userElement;
                        else if (root.TryGetProperty("Data", out var dataElement2))
                            userData = dataElement2;
                        else if (root.TryGetProperty("User", out var userElement2))
                            userData = userElement2;
                        
                        var id = GetGuidProperty(userData, "id", "Id", "userId", "UserId");
                        var username = GetStringProperty(userData, "username", "Username", "userName", "UserName");
                        var email = GetStringProperty(userData, "email", "Email");
                        var fullName = GetStringProperty(userData, "fullName", "FullName", "full_name");
                        var roleId = GetNullableGuidProperty(userData, "roleId", "RoleId", "role_id");
                        var roleName = GetStringProperty(userData, "roleName", "RoleName", "role_name");
                        
                        _logger.LogInformation("Creating user in MongoDB: {Username} ({Id})", username, id);
                        
                        var userModel = new UserReadModel
                        {
                            Id = id,
                            Username = username,
                            Email = email,
                            FullName = fullName,
                            RoleId = roleId,
                            RoleName = roleName,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        await dao.UpsertAsync(userModel);
                        _logger.LogInformation("User created in MongoDB: {Username}", username);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse UserCreatedEvent");
                        throw;
                    }
                    break;
                }
                case "auth.user.updated":
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(actualPayload);
                        var root = doc.RootElement;
                        
                        var userId = GetGuidProperty(root, "id", "Id", "userId", "UserId");
                        var username = GetStringProperty(root, "username", "Username", "userName", "UserName");
                        var email = GetStringProperty(root, "email", "Email");
                        var fullName = GetStringProperty(root, "fullName", "FullName", "full_name");
                        var roleId = GetNullableGuidProperty(root, "roleId", "RoleId", "role_id");
                        var roleName = GetStringProperty(root, "roleName", "RoleName", "role_name");
                        
                        _logger.LogInformation("Updating user in MongoDB: {Id}", userId);
                        
                        var existing = await dao.GetByIdAsync(userId);
                        if (existing is null)
                        {
                            _logger.LogWarning("⚠User not found in MongoDB for update: {Id}", userId);
                            break;
                        }
                        
                        if (!string.IsNullOrEmpty(username)) existing.Username = username;
                        if (!string.IsNullOrEmpty(email)) existing.Email = email;
                        if (!string.IsNullOrEmpty(fullName)) existing.FullName = fullName;
                        if (roleId.HasValue) existing.RoleId = roleId;
                        if (!string.IsNullOrEmpty(roleName)) existing.RoleName = roleName;
                        
                        await dao.UpsertAsync(existing);
                        _logger.LogInformation("User updated in MongoDB: {Id}", userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse UserUpdatedEvent");
                        throw;
                    }
                    break;
                }
                case "auth.user.profile.updated":
                case "auth.user.password.changed":
                case "auth.user.password.reset":
                {
                    // These events update user data, so we need to update the MongoDB document
                    try
                    {
                        using var doc = JsonDocument.Parse(actualPayload);
                        var root = doc.RootElement;
                        
                        // Extract userId (could be "userId" or "id")
                        var userId = GetGuidProperty(root, "id", "Id", "userId", "UserId");
                        
                        _logger.LogInformation("Updating user in MongoDB for event {Type}: {Id}", type, userId);
                        
                        var existing = await dao.GetByIdAsync(userId);
                        if (existing is null)
                        {
                            _logger.LogWarning("⚠User not found in MongoDB for {Type}: {Id}", type, userId);
                            break;
                        }
                        
                        // For profile.updated, update fullName
                        if (type == "auth.user.profile.updated")
                        {
                            var fullName = GetStringProperty(root, "fullName", "FullName", "full_name");
                            if (!string.IsNullOrEmpty(fullName)) existing.FullName = fullName;
                        }
                        
                        // For password events, we don't need to update anything in the read model
                        // (passwords are not stored in read model for security)
                        // But we still update to ensure the document exists
                        
                        await dao.UpsertAsync(existing);
                        _logger.LogInformation("User updated in MongoDB for {Type}: {Id}", type, userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse {Type} event", type);
                        throw;
                    }
                    break;
                }
                case "auth.user.deleted":
                {
                    var e = JsonSerializer.Deserialize<UserDeletedEvent>(payload, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    })!;
                    
                    _logger.LogInformation("Deleting user from MongoDB: {Id}", e.id);
                    await dao.DeleteAsync(e.id);
                    _logger.LogInformation("User deleted from MongoDB: {Id}", e.id);
                    break;
                }
                default:
                    _logger.LogWarning("Unknown event type: {Type}", type);
                    break;
            }
        }

        private static Guid GetGuidProperty(JsonElement element, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                if (element.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.String && Guid.TryParse(prop.GetString(), out var guid))
                        return guid;
                }
            }
            throw new JsonException($"Required Guid property not found. Tried: {string.Join(", ", propertyNames)}");
        }

        private static Guid? GetNullableGuidProperty(JsonElement element, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                if (element.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.String && Guid.TryParse(prop.GetString(), out var guid))
                        return guid;
                    if (prop.ValueKind == JsonValueKind.Null)
                        return null;
                }
            }
            return null;
        }

        private static string GetStringProperty(JsonElement element, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                if (element.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                {
                    return prop.GetString() ?? string.Empty;
                }
            }
            throw new JsonException($"Required string property not found. Tried: {string.Join(", ", propertyNames)}");
        }

        private sealed record UserCreatedEvent(Guid id, string username, string email, string? fullName, Guid? roleId, DateTimeOffset occurredAt);
        private sealed record UserUpdatedEvent(Guid id, string? username, string? email, string? fullName, Guid? roleId, DateTimeOffset occurredAt);
        private sealed record UserDeletedEvent(Guid id, DateTimeOffset occurredAt);

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try { if (_channel is not null) await _channel.CloseAsync(cancellationToken); } catch { }
            try { if (_conn is not null) await _conn.CloseAsync(cancellationToken); } catch { }
            await base.StopAsync(cancellationToken);
        }
    }
}