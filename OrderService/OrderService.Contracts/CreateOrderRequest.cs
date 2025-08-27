namespace OrderService.OrderService.Contracts
{
    public record CreateOrderRequest(List<CreateOrderItemRequest> Items);

}
