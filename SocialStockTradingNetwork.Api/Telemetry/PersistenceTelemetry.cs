using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SocialStockTradingNetwork.Api.Telemetry;

internal static class PersistenceTelemetry
{
    internal const string SourceName = "socialstocks.persistence";

    internal static readonly ActivitySource ActivitySource = new(SourceName);

    private static readonly Meter Meter = new(SourceName);

    internal static readonly Counter<long> OperationsCount =
        Meter.CreateCounter<long>(
            "db.operations.count",
            description: "Number of persistence operations executed");

    internal static readonly Histogram<double> OperationDuration =
        Meter.CreateHistogram<double>(
            "db.operation.duration.ms",
            unit: "ms",
            description: "Duration of persistence operations");
}
