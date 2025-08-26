namespace OrderService.OrderService.Messaging
{
    public interface IMessageBusClient
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default);
    }
}
