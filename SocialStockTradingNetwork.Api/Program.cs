using SocialStockTradingNetwork.Api.Endpoints;
using SocialStockTradingNetwork.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddPersistence(builder.Configuration);

// When adding OpenTelemetry exporters, wire the meter and activity sources:
//   builder.Services.AddOpenTelemetry()
//       .WithMetrics(m => m.AddMeter(StocksTelemetry.SourceName, PersistenceTelemetry.SourceName))
//       .WithTracing(t => t.AddSource(StocksTelemetry.SourceName, PersistenceTelemetry.SourceName));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapStocksEndpoints();

app.Run();
