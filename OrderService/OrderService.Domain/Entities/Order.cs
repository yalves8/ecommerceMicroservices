using OrderService.OrderService.Domain.Enum;

namespace OrderService.OrderService.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
