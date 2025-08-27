using OrderService.OrderService.Domain.Entities;

namespace OrderService.OrderService.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(List<(string productId, int quantity)> items);
        Task<Order?> GetOrderByIdAsync(int id);
    }
}
