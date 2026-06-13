using Microsoft.EntityFrameworkCore;
using SocialStockTradingNetwork.Api.Repositories;

namespace SocialStockTradingNetwork.Api.Persistence;

internal static class PersistenceServiceCollectionExtensions
{
    private const string ConnectionStringName = "Default";

    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgres", tags: ["ready"]);

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
