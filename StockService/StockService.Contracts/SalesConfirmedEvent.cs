namespace StockService.Contracts
{
    public class SalesConfirmedEvent
    {
        public Guid OrderId { get; set; }
        public List<SalesItem> Items { get; set; } = new();
    }
}
