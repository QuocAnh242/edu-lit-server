using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AuthService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AuthService.Infrastructure.Messaging
{
    public sealed class RabbitMqRoleProjectionService : BackgroundService
    {
        private readonly ILogger<RabbitMqRoleProjectionService> _logger;
        private readonly IServiceProvider _sp;
        private readonly ConnectionFactory _factory;
        private IConnection? _conn;
        private IChannel? _channel;

        private const string Exchange = "auth.users";
        private const string QueueName = "auth-service-query.roles";

        public RabbitMqRoleProjectionService(
            IConfiguration cfg,
            ILogger<RabbitMqRoleProjectionService> logger,
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
                _logger.LogInformation("Starting RabbitMQ Role Projection Service...");
                _logger.LogInformation("Connecting to RabbitMQ: {HostName}:{Port}", _factory.HostName, _factory.Port);

                // Retry connection with delay
                var retryCount = 0;
                while (retryCount < 10)
                {
                    try
                    {
                        _conn = await _factory.CreateConnectionAsync(clientProvidedName: "auth-service-query-roles", cancellationToken: stoppingToken);
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

                foreach (var rk in new[] { "auth.role.created", "auth.role.updated", "auth.role.deleted" })
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
                        
                        _logger.LogInformation("Received role event: {RoutingKey}", routingKey);
                        _logger.LogDebug("Payload: {Payload}", json);
                        
                        await ProjectAsync(routingKey, json, stoppingToken);
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                        
                        _logger.LogInformation("Role event processed successfully: {RoutingKey}", routingKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Role projection failed for event. Nacking message.");
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                    }
                };

                await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
                _logger.LogInformation("RabbitMQ role projection service is now listening for events...");
                _logger.LogInformation("Service started successfully!");

                // Keep running until cancellation
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Role projection service is stopping...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in RabbitMQ role projection service");
                throw;
            }
            finally
            {
                if (_channel != null)
                {
                    await _channel.CloseAsync();
                    _channel.Dispose();
                }
                if (_conn != null)
                {
                    await _conn.CloseAsync();
                    _conn.Dispose();
                }
            }
        }

        private async Task ProjectAsync(string type, string payload, CancellationToken ct)
        {
            _logger.LogInformation("Projecting role event type: {Type}", type);
            
            // Handle double-serialized JSON (payload might be a JSON string containing JSON)
            var actualPayload = payload;
            try
            {
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
            var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            var rolesCollection = mongoDatabase.GetCollection<UserRole>("roles_read");

            switch (type)
            {
                case "auth.role.created":
                {
                    try
                    {
                        _logger.LogInformation("Raw payload: {Payload}", actualPayload);
                        using var doc = JsonDocument.Parse(actualPayload);
                        var root = doc.RootElement;
                        
                        // Log all properties to debug
                        _logger.LogInformation("Root element properties: {Properties}", string.Join(", ", root.EnumerateObject().Select(p => p.Name)));
                        
                        var id = GetGuidProperty(root, "id", "Id", "roleId", "RoleId");
                        var name = GetStringProperty(root, "name", "Name");
                        
                        if (id == Guid.Empty)
                        {
                            _logger.LogError("Failed to extract role ID from payload. Payload: {Payload}", actualPayload);
                            // Try to get ID from the role object if nested
                            if (root.TryGetProperty("role", out var roleElement))
                            {
                                id = GetGuidProperty(roleElement, "id", "Id");
                            }
                        }
                        
                        if (string.IsNullOrEmpty(name))
                        {
                            _logger.LogError("Invalid role data - Name is empty. Skipping.");
                            break;
                        }
                        
                        // If ID is empty, we'll still create the role with name
                        // The role will be identified by name, and we'll generate a new ID for MongoDB
                        if (id == Guid.Empty)
                        {
                            _logger.LogWarning("Role ID is empty for role {Name}. Generating new ID for MongoDB.", name);
                            id = Guid.NewGuid();
                        }
                        
                        _logger.LogInformation("Creating role in MongoDB: {Name} ({Id})", name, id);
                        
                        var role = new UserRole
                        {
                            Id = id,
                            Name = name
                        };
                        
                        // Use name as unique identifier instead of ID (in case ID is wrong)
                        var filter = Builders<UserRole>.Filter.Eq(r => r.Name, name);
                        await rolesCollection.ReplaceOneAsync(filter, role, new ReplaceOptions { IsUpsert = true }, cancellationToken: ct);
                        
                        _logger.LogInformation("Role created/updated in MongoDB: {Name} ({Id})", name, id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse RoleCreatedEvent. Payload: {Payload}", actualPayload);
                        throw;
                    }
                    break;
                }
                case "auth.role.updated":
                {
                    try
                    {
                        _logger.LogInformation("Raw payload: {Payload}", actualPayload);
                        using var doc = JsonDocument.Parse(actualPayload);
                        var root = doc.RootElement;
                        
                        var id = GetGuidProperty(root, "id", "Id", "roleId", "RoleId");
                        var name = GetStringProperty(root, "name", "Name");
                        
                        if (id == Guid.Empty)
                        {
                            _logger.LogError("Failed to extract role ID from payload. Payload: {Payload}", actualPayload);
                            if (root.TryGetProperty("role", out var roleElement))
                            {
                                id = GetGuidProperty(roleElement, "id", "Id");
                            }
                        }
                        
                        if (id == Guid.Empty || string.IsNullOrEmpty(name))
                        {
                            _logger.LogError("Invalid role data - ID: {Id}, Name: {Name}. Skipping.", id, name);
                            break;
                        }
                        
                        _logger.LogInformation("Updating role in MongoDB: {Name} ({Id})", name, id);
                        
                        var role = new UserRole
                        {
                            Id = id,
                            Name = name
                        };
                        
                        // Use name as unique identifier to ensure we update the correct role
                        var filter = Builders<UserRole>.Filter.Eq(r => r.Name, name);
                        await rolesCollection.ReplaceOneAsync(filter, role, new ReplaceOptions { IsUpsert = true }, cancellationToken: ct);
                        
                        _logger.LogInformation("Role updated in MongoDB: {Name} ({Id})", name, id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse RoleUpdatedEvent. Payload: {Payload}", actualPayload);
                        throw;
                    }
                    break;
                }
                case "auth.role.deleted":
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(actualPayload);
                        var root = doc.RootElement;
                        
                        var id = GetGuidProperty(root, "id", "Id", "roleId", "RoleId");
                        
                        _logger.LogInformation("Deleting role from MongoDB: {Id}", id);
                        
                        var filter = Builders<UserRole>.Filter.Eq(r => r.Id, id);
                        await rolesCollection.DeleteOneAsync(filter, cancellationToken: ct);
                        
                        _logger.LogInformation("Role deleted from MongoDB: {Id}", id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse RoleDeletedEvent");
                        throw;
                    }
                    break;
                }
                default:
                    _logger.LogWarning("Unknown role event type: {Type}", type);
                    break;
            }
        }

        private Guid GetGuidProperty(JsonElement element, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                if (element.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.String)
                    {
                        var str = prop.GetString();
                        if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var guid))
                            return guid;
                    }
                    else if (prop.ValueKind == JsonValueKind.Object)
                    {
                        // Try to get value from nested object
                        if (prop.TryGetProperty("value", out var valueProp) && valueProp.ValueKind == JsonValueKind.String)
                        {
                            var str = valueProp.GetString();
                            if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var guid2))
                                return guid2;
                        }
                    }
                    else if (prop.ValueKind == JsonValueKind.Array && prop.GetArrayLength() > 0)
                    {
                        // Sometimes GUIDs are serialized as arrays
                        var first = prop[0];
                        if (first.ValueKind == JsonValueKind.String)
                        {
                            var str = first.GetString();
                            if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var guid3))
                                return guid3;
                        }
                    }
                }
            }
            // If not found, log warning and return empty GUID
            _logger.LogWarning("Required Guid property not found. Tried: {PropertyNames}. Returning empty GUID.", string.Join(", ", propertyNames));
            return Guid.Empty;
        }

        private static string GetStringProperty(JsonElement element, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                if (element.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.String)
                        return prop.GetString() ?? string.Empty;
                }
            }
            return string.Empty;
        }
    }
}

