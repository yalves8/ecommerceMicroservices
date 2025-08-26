using OrderService.OrderService.Contracts.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.OrderService.Messaging
{
    public class RabbitMQMessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMQMessageBusClient(IConfiguration configuration)
        {
            var host = configuration["RabbitMQ:Host"] ?? "rabbitmq"; // docker: rabbitmq, local: localhost
            var factory = new ConnectionFactory { HostName = host };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            _channel.ExchangeDeclareAsync(
                exchange: "order_exchange",
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null
            ).GetAwaiter().GetResult();

        }

        public async Task PublishOrderConfirmed(SalesConfirmedEvent evt)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "sales-confirmed",
                body: body
            );
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
