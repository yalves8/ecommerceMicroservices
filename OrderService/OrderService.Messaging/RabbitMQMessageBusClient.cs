using OrderService.OrderService.Contracts.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.OrderService.Messaging
{
    public class RabbitMQMessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly ILogger<RabbitMQMessageBusClient> _logger;
        private readonly IConfiguration _config;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQMessageBusClient(ILogger<RabbitMQMessageBusClient> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            var host = _config["RabbitMQ:Host"] ?? "localhost";
            var factory = new ConnectionFactory { HostName = host };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            var queue = _config["RabbitMQ:Queue"] ?? "sales-confirmed";

            _channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false
            ).GetAwaiter().GetResult();
        }

        public async Task PublishSalesConfirmedAsync(SalesConfirmedEvent evt, CancellationToken ct = default)
        {
            var queue = _config["RabbitMQ:Queue"] ?? "sales-confirmed";

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));

            await _channel!.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                body: body
            );

            _logger.LogInformation("SalesConfirmedEvent published to queue {queue}", queue);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
