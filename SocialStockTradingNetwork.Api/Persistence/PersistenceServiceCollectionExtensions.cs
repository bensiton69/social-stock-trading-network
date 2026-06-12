using Microsoft.EntityFrameworkCore;
using SocialStockTradingNetwork.Api.Repositories;

namespace SocialStockTradingNetwork.Api.Persistence;

internal static class PersistenceServiceCollectionExtensions
{
    // Matches the Aspire database resource name defined in AppHost — Aspire injects
    // ConnectionStrings__socialstocks at runtime, and dotnet ef falls back to
    // appsettings.Development.json when running migrations outside the orchestrator.
    private const string ConnectionStringName = "socialstocks";

    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
