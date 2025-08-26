namespace OrderService.OrderService.Contracts.Events
{
    public class SalesConfirmedEvent
    {
        public Guid OrderId { get; set; }
        public List<SalesConfirmedItem> Items { get; set; } = new();
    }
}
