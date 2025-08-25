namespace StockService.Domain
{
    public class Product
    {
        public int Id { get; set; }                         // identity
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }                  // decimal(18,2)
        public int Quantity { get; set; }                 // estoque atual
    }
}
