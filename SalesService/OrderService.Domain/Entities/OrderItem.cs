namespace OrderService.OrderService.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
