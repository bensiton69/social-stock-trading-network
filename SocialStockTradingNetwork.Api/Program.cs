using SocialStockTradingNetwork.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// When adding OpenTelemetry exporters, wire StocksTelemetry.SourceName as the meter and activity source:
//   builder.Services.AddOpenTelemetry()
//       .WithMetrics(m => m.AddMeter(StocksTelemetry.SourceName))
//       .WithTracing(t => t.AddSource(StocksTelemetry.SourceName));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapStocksEndpoints();

app.Run();
