using OrderService.OrderService.Domain.Entities;
using OrderService.OrderService.Domain.Enum;
using OrderService.OrderService.Infrastructure.Repositories;

namespace OrderService.OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Order> CreateOrderAsync(List<(string productId, int quantity)> items)
        {
            var order = new Order
            {
                Status = OrderStatus.Pending,
                Items = items.Select(i => new OrderItem
                {
                    ProductId = i.productId,
                    Quantity = i.quantity
                }).ToList()
            };

            await _repository.AddAsync(order);
            await _repository.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
