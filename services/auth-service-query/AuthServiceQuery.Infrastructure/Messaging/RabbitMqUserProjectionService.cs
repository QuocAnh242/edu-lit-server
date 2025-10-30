using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AuthService.Infrastructure.DAO.Interfaces;
using AuthService.Domain.Entities.ReadModels;

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
            _conn = await _factory.CreateConnectionAsync(clientProvidedName: "auth-service-query", cancellationToken: stoppingToken);
            _channel = await _conn.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(Exchange, ExchangeType.Topic, durable: true, autoDelete: false, arguments: null, cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            foreach (var rk in new[] { "auth.user.created", "auth.user.updated", "auth.user.deleted" })
                await _channel.QueueBindAsync(QueueName, Exchange, rk, arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var routingKey = ea.RoutingKey;
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    await ProjectAsync(routingKey, json, stoppingToken);
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Projection failed. Nacking message.");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
            _logger.LogInformation("RabbitMQ user projection started.");
        }

        private async Task ProjectAsync(string type, string payload, CancellationToken ct)
        {
            await using var scope = _sp.CreateAsyncScope();
            var dao = scope.ServiceProvider.GetRequiredService<IUserReadDAO>();

            switch (type)
            {
                case "auth.user.created":
                {
                    var e = JsonSerializer.Deserialize<UserCreatedEvent>(payload)!;
                    await dao.UpsertAsync(new UserReadModel
                    {
                        Id = e.id,
                        Username = e.username,
                        Email = e.email,
                        FullName = e.fullName,
                        RoleId = e.roleId,
                        CreatedAt = e.occurredAt.UtcDateTime
                    });
                    break;
                }
                case "auth.user.updated":
                {
                    var e = JsonSerializer.Deserialize<UserUpdatedEvent>(payload)!;
                    var existing = await dao.GetByIdAsync(e.id);
                    if (existing is null) break;
                    existing.Username = e.username ?? existing.Username;
                    existing.Email = e.email ?? existing.Email;
                    existing.FullName = e.fullName ?? existing.FullName;
                    existing.RoleId = e.roleId ?? existing.RoleId;
                    await dao.UpsertAsync(existing);
                    break;
                }
                case "auth.user.deleted":
                {
                    var e = JsonSerializer.Deserialize<UserDeletedEvent>(payload)!;
                    await dao.DeleteAsync(e.id);
                    break;
                }
                default:
                    _logger.LogWarning("Unknown event type {Type}", type);
                    break;
            }
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