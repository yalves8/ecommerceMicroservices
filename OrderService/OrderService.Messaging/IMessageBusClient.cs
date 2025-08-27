using OrderService.OrderService.Contracts.Events;

namespace OrderService.OrderService.Messaging
{
    public interface IMessageBusClient
    {
        Task PublishOrderConfirmed(SalesConfirmedEvent evt);
    }
}
