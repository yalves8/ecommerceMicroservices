using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockService.Contracts;
using StockService.Infrastructure;

namespace StockService.Messaging
{
    public class SalesEventsConsumer : BackgroundService
    {
        private readonly ILogger<SalesEventsConsumer> _logger;
        private readonly IServiceProvider _sp;
        private readonly IConfiguration _config;
        private IConnection? _conn;
        private IChannel? _ch;

        public SalesEventsConsumer(ILogger<SalesEventsConsumer> logger, IServiceProvider sp, IConfiguration config)
            => (_logger, _sp, _config) = (logger, sp, config);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var host = _config["RabbitMQ:Host"] ?? "rabbitmq";    // docker: "rabbitmq", local: "localhost"
            var queue = _config["RabbitMQ:Queue"] ?? "sales-confirmed";

            var factory = new ConnectionFactory { HostName = host };
            _conn = await factory.CreateConnectionAsync(stoppingToken);

            _ch = await _conn.CreateChannelAsync(null, stoppingToken);

            await _ch.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_ch);
  
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var ct = stoppingToken;

                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    _logger.LogInformation("Received message from queue '{queue}': {json}", queue, json);
                    var evt = JsonSerializer.Deserialize<SalesConfirmedEvent>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (evt?.Items is null || evt.Items.Count == 0)
                    {
                        await _ch.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: ct);
                        return;
                    }

                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();

                    foreach (var item in evt.Items)
                    {
                        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken: ct);
                        if (product is null) continue;
                        product.Quantity = Math.Max(0, product.Quantity - item.Quantity);
                    }

                    await db.SaveChangesAsync(ct);

                    //comment line to observe the message requeue
                    await _ch.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing sales message");
                    await _ch!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: ct);
                }
            };

            await _ch.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);
            await _ch.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
            _logger.LogInformation("Sales Consumer Started. Queue: {queue}", queue);
        }

        public override void Dispose()
        {
            _ch?.Dispose();
            _conn?.Dispose();
            base.Dispose();
        }

    }
}
