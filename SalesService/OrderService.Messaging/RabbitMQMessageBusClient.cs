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

            // cria a conexão e o canal de forma assíncrona
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

        public async Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default)
        {
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // Chama a versão assíncrona, mas espera o término para manter sincronia
            _channel.BasicPublishAsync(
                exchange: "order_exchange",
                routingKey: queueName,
                mandatory: false,
                body: body,
                cancellationToken: ct
            ).GetAwaiter().GetResult();

            Console.WriteLine($"Event Published -> {json}");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
