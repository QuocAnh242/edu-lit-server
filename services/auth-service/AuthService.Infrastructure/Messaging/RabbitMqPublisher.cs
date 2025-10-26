using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace AuthService.Infrastructure.Messaging
{
    public sealed class RabbitMqPublisher : IMessageBusPublisher
    {
        private readonly IConnection _connection;
        public RabbitMqPublisher(IConnection connection) => _connection = connection;

        public async Task PublishAsync(string type, string payload, CancellationToken ct = default)
        {
            const string exchange = "auth.users";

            await using var channel = await _connection.CreateChannelAsync(cancellationToken: ct);

            await channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct);

            var body = Encoding.UTF8.GetBytes(payload);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: type,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct);
        }
    }
}