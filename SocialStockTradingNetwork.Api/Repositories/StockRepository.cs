using Microsoft.EntityFrameworkCore;
using SocialStockTradingNetwork.Api.Entities;
using SocialStockTradingNetwork.Api.Persistence;

namespace SocialStockTradingNetwork.Api.Repositories;

public sealed class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default) =>
        InstrumentAsync("get_by_symbol", () =>
            Set.AsNoTracking().FirstOrDefaultAsync(stock => stock.Symbol == symbol, cancellationToken));
}
