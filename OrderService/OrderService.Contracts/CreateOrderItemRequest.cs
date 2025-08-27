namespace OrderService.OrderService.Contracts
{
    public record CreateOrderItemRequest(int ProductId, int Quantity);
}
