using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SocialStockTradingNetwork.Api.Telemetry;

internal static class StocksTelemetry
{
    internal const string SourceName = "socialstocks.stocks";

    internal static readonly ActivitySource ActivitySource = new(SourceName);

    private static readonly Meter Meter = new(SourceName);

    internal static readonly Counter<long> StocksCreatedCount =
        Meter.CreateCounter<long>(
            "stocks.created.count",
            description: "Number of stock create requests processed");

    internal static readonly Histogram<double> StocksCreateDuration =
        Meter.CreateHistogram<double>(
            "stocks.create.duration.ms",
            unit: "ms",
            description: "Duration of stock create request processing");
}
