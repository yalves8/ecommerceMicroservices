using OrderService.OrderService.Domain.Entities;

namespace OrderService.OrderService.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task SaveChangesAsync();
    }
}
