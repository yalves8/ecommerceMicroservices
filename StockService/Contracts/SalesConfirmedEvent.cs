namespace StockService.Contracts
{
    public class SalesConfirmedEvent
    {
        public string? OrderId { get; set; }
        public List<SalesItem> Items { get; set; } = new();
    }
}
