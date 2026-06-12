namespace SocialStockTradingNetwork.Api.Dtos;

public class CreateStockRequest
{
    public required string Symbol { get; init; }
    public required string Name { get; init; }
    public decimal CurrentPrice { get; init; }
    public required string Currency { get; init; }
    public bool IsTradable { get; init; }
}
