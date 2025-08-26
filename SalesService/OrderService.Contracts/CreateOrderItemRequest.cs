namespace OrderService.OrderService.Contracts
{
    public class CreateOrderItemRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
