using OrderService.OrderService.Contracts.Events;

namespace OrderService.OrderService.Messaging
{
    public interface IMessageBusClient
    {
        Task PublishSalesConfirmedAsync(SalesConfirmedEvent evt, CancellationToken ct = default);
    }
}
