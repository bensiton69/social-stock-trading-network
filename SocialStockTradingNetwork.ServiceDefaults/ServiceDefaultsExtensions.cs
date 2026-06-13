using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

public static class ServiceDefaultsExtensions
{
    /// <summary>
    /// Wires up Aspire service defaults: OpenTelemetry (traces, metrics, logs),
    /// health checks, and service discovery. Pass domain meter/source names so
    /// the pipeline captures custom instrumentation.
    /// </summary>
    public static TBuilder AddServiceDefaults<TBuilder>(
        this TBuilder builder,
        string[]? additionalMeters = null,
        string[]? additionalActivitySources = null)
        where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry(additionalMeters, additionalActivitySources);
        builder.AddDefaultHealthChecks();
        builder.Services.AddServiceDiscovery();

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(
        this TBuilder builder,
        string[]? additionalMeters = null,
        string[]? additionalActivitySources = null)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (additionalMeters?.Length > 0)
                    metrics.AddMeter(additionalMeters);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (additionalActivitySources?.Length > 0)
                    tracing.AddSource(additionalActivitySources);
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // OTLP exporter is activated when the endpoint env-var is present.
        // Aspire Dashboard injects OTEL_EXPORTER_OTLP_ENDPOINT automatically
        // during local dev; in AWS (ECS/EKS) point it at your collector side-car.
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            builder.Services.AddOpenTelemetry().UseOtlpExporter();

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    /// <summary>
    /// Maps /health (full readiness, includes dependency checks) and /alive
    /// (liveness only). Exposed in all environments so container/Kubernetes
    /// probes can reach them in production as well as local dev.
    /// </summary>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");

        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }
}
