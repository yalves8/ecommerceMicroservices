using OrderService.OrderService.Domain.Entities;
using OrderService.OrderService.Domain.Enum;
using OrderService.OrderService.Infrastructure.Repositories;
using OrderService.OrderService.Messaging;
using StockService.Contracts;

namespace OrderService.OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMessageBusClient _messageBus;

        public OrderService(IOrderRepository repository, IMessageBusClient messageBus)
        {
            _repository = repository;
            _messageBus = messageBus;
        }

        public Task<Order> CreateOrderAsync(List<(string productId, int quantity)> items)
        {
            throw new NotImplementedException();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
