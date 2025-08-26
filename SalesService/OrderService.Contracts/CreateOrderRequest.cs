namespace OrderService.OrderService.Contracts
{
    public class CreateOrderRequest
    {
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

}
