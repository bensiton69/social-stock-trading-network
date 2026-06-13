using SocialStockTradingNetwork.Api.Endpoints;
using SocialStockTradingNetwork.Api.Persistence;
using SocialStockTradingNetwork.Api.Telemetry;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(
    additionalMeters: [StocksTelemetry.SourceName, PersistenceTelemetry.SourceName],
    additionalActivitySources: [StocksTelemetry.SourceName, PersistenceTelemetry.SourceName]);

builder.Services.AddOpenApi();

builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

await app.ApplyDatabaseMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// TLS is terminated upstream (load balancer) in container/Kubernetes
// deployments, so HTTPS redirection is opt-out via configuration.
if (builder.Configuration.GetValue("ENABLE_HTTPS_REDIRECTION", defaultValue: true))
{
    app.UseHttpsRedirection();
}

app.MapDefaultEndpoints();
app.MapStocksEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Social Stock Trading Network API v1");
    });
}

app.Run();
