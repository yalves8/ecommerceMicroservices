namespace OrderService.OrderService.Contracts
{
    public record CreateOrderItemRequest(Guid ProductId, int Quantity);
}
