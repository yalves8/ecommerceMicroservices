namespace StockService.Contracts
{
    public class SalesConfirmedEvent
    {
        public int OrderId { get; set; }
        public List<SalesItem> Items { get; set; } = new();
    }
}
