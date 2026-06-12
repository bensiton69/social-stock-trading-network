using SocialStockTradingNetwork.Api.Dtos;
using SocialStockTradingNetwork.Api.Entities;

namespace SocialStockTradingNetwork.Api.Mappers;

internal static class StockComposition
{
    internal static Stock ToEntity(CreateStockRequest request) =>
        new()
        {
            Id = Guid.NewGuid(),
            Symbol = request.Symbol,
            Name = request.Name,
            CurrentPrice = request.CurrentPrice,
            Currency = request.Currency,
            IsTradable = request.IsTradable,
            UpdatedAt = DateTime.UtcNow
        };

    internal static StockDto ToDto(Stock stock) =>
        new()
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            Name = stock.Name,
            CurrentPrice = stock.CurrentPrice,
            Currency = stock.Currency,
            IsTradable = stock.IsTradable,
            UpdatedAt = stock.UpdatedAt
        };

    internal static IReadOnlyList<StockDto> ToDtoList(IReadOnlyList<Stock> stocks) =>
        [.. stocks.Select(ToDto)];
}
