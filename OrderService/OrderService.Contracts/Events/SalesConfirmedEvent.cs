namespace OrderService.OrderService.Contracts.Events
{
    public class SalesConfirmedEvent
    {
        public int OrderId { get; set; }
        public List<SalesConfirmedItem> Items { get; set; } = new();
    }
}
