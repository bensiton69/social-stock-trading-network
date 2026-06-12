using SocialStockTradingNetwork.Api.Entities;

namespace SocialStockTradingNetwork.Api.Repositories;

public interface IStockRepository : IRepository<Stock>
{
    Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
}
