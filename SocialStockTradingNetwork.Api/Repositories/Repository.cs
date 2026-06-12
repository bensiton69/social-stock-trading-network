using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SocialStockTradingNetwork.Api.Persistence;
using SocialStockTradingNetwork.Api.Telemetry;

namespace SocialStockTradingNetwork.Api.Repositories;

public class Repository<T> : IRepository<T>
    where T : class
{
    private readonly DbSet<T> _set;

    public Repository(AppDbContext dbContext)
    {
        _set = dbContext.Set<T>();
    }

    protected DbSet<T> Set => _set;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        InstrumentAsync("get_by_id", () => _set.FindAsync([id], cancellationToken).AsTask());

    public Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default) =>
        InstrumentAsync<IReadOnlyList<T>>("list", async () => await _set.AsNoTracking().ToListAsync(cancellationToken));

    public Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        InstrumentAsync("add", async () =>
        {
            await _set.AddAsync(entity, cancellationToken);
            return true;
        });

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    protected static async Task<TResult> InstrumentAsync<TResult>(string operation, Func<Task<TResult>> action)
    {
        var entityName = typeof(T).Name;
        using var activity = PersistenceTelemetry.ActivitySource.StartActivity(
            $"db.{entityName}.{operation}",
            ActivityKind.Client);
        activity?.SetTag("db.entity", entityName)
                 .SetTag("db.operation", operation);

        var start = Stopwatch.GetTimestamp();
        try
        {
            var result = await action();
            RecordMetrics(entityName, operation, "success", start);
            return result;
        }
        catch (Exception ex)
        {
            activity?.AddEvent(new ActivityEvent("exception"));
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            RecordMetrics(entityName, operation, "failure", start);
            throw;
        }
    }

    private static void RecordMetrics(string entityName, string operation, string status, long start)
    {
        var tags = new TagList
        {
            { "db.entity", entityName },
            { "db.operation", operation },
            { "status", status }
        };

        PersistenceTelemetry.OperationsCount.Add(1, tags);
        PersistenceTelemetry.OperationDuration.Record(Stopwatch.GetElapsedTime(start).TotalMilliseconds, tags);
    }
}
