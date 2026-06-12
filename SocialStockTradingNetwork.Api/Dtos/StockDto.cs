namespace SocialStockTradingNetwork.Api.Dtos;

public class StockDto
{
    public Guid Id { get; set; }
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public decimal CurrentPrice { get; set; }
    public required string Currency { get; set; }
    public bool IsTradable { get; set; }
    public DateTime UpdatedAt { get; set; }
}
