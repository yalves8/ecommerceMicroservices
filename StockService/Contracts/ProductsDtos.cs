namespace StockService.Contracts
{
    public record CreateProductDto(string Name, string? Description, decimal Price, int Quantity);
    public record UpdateStockDto(int Quantity);
    public record ProductResponse(int Id, string Name, string? Description, decimal Price, int Quantity);
}
