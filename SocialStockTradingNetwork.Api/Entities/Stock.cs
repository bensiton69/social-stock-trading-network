namespace SocialStockTradingNetwork.Api.Entities;

public class Stock
{
    public Guid Id { get; set; }
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public decimal CurrentPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsTradable { get; set; } = true;
    public DateTime UpdatedAt { get; set; }
}
