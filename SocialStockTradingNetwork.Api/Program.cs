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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();
app.MapStocksEndpoints();

app.Run();
