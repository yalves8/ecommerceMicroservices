namespace OrderService.OrderService.Contracts.Events
{
    public class SalesConfirmedItem
    {
        //TODO colocar um projeto prara compartilhar os contratos
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
