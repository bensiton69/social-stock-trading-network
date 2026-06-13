using Microsoft.EntityFrameworkCore;

namespace SocialStockTradingNetwork.Api.Persistence;

internal static class DatabaseMigrationExtensions
{
    private const string RunMigrationsKey = "RUN_MIGRATIONS";

    /// <summary>
    /// Applies pending EF Core migrations at startup when RUN_MIGRATIONS is
    /// enabled. Used by the single-replica container/Kubernetes deployment so
    /// the schema is created without a separate migration job.
    /// </summary>
    internal static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue(RunMigrationsKey, defaultValue: false))
            return;

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
