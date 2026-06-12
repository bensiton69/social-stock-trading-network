using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using SocialStockTradingNetwork.Api.Dtos;
using SocialStockTradingNetwork.Api.Mappers;
using SocialStockTradingNetwork.Api.Repositories;
using SocialStockTradingNetwork.Api.Telemetry;

namespace SocialStockTradingNetwork.Api.Endpoints;

internal static class StocksEndpoints
{
    internal static IEndpointRouteBuilder MapStocksEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/stocks").WithTags("Stocks");

        group.MapPost("/", CreateStock)
            .WithName("CreateStock")
            .WithSummary("Create a new stock")
            .Produces<StockDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/{id:guid}", GetStockById)
            .WithName("GetStockById")
            .WithSummary("Get a stock by its identifier")
            .Produces<StockDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", GetStocks)
            .WithName("GetStocks")
            .WithSummary("List all stocks")
            .Produces<IReadOnlyList<StockDto>>();

        return app;
    }

    private static async Task<Results<Created<StockDto>, ValidationProblem, Conflict>> CreateStock(
        CreateStockRequest request,
        IStockRepository stocks,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            StocksTelemetry.StocksCreatedCount.Add(1, new KeyValuePair<string, object?>("status", "failure"));
            return TypedResults.ValidationProblem(validationErrors);
        }

        using var activity = StocksTelemetry.ActivitySource.StartActivity("stocks.create", ActivityKind.Internal);
        var start = Stopwatch.GetTimestamp();

        var existing = await stocks.GetBySymbolAsync(request.Symbol, cancellationToken);
        if (existing is not null)
        {
            StocksTelemetry.StocksCreatedCount.Add(1, new KeyValuePair<string, object?>("status", "conflict"));
            return TypedResults.Conflict();
        }

        var stock = StockComposition.ToEntity(request);
        await stocks.AddAsync(stock, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = StockComposition.ToDto(stock);

        var elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
        StocksTelemetry.StocksCreateDuration.Record(elapsedMs, new KeyValuePair<string, object?>("status", "success"));
        StocksTelemetry.StocksCreatedCount.Add(1, new KeyValuePair<string, object?>("status", "success"));

        activity?.SetTag("stock.id", stock.Id)
                 .SetTag("stock.symbol", stock.Symbol);

        return TypedResults.Created($"/stocks/{stock.Id}", dto);
    }

    private static async Task<Results<Ok<StockDto>, NotFound>> GetStockById(
        Guid id,
        IStockRepository stocks,
        CancellationToken cancellationToken)
    {
        var stock = await stocks.GetByIdAsync(id, cancellationToken);
        return stock is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(StockComposition.ToDto(stock));
    }

    private static async Task<Ok<IReadOnlyList<StockDto>>> GetStocks(
        IStockRepository stocks,
        CancellationToken cancellationToken)
    {
        var entities = await stocks.ListAsync(cancellationToken);
        return TypedResults.Ok(StockComposition.ToDtoList(entities));
    }

    private static Dictionary<string, string[]> Validate(CreateStockRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Symbol))
            errors[nameof(request.Symbol)] = ["Symbol is required."];

        if (string.IsNullOrWhiteSpace(request.Name))
            errors[nameof(request.Name)] = ["Name is required."];

        if (string.IsNullOrWhiteSpace(request.Currency))
            errors[nameof(request.Currency)] = ["Currency is required."];

        if (request.CurrentPrice < 0)
            errors[nameof(request.CurrentPrice)] = ["CurrentPrice must be zero or greater."];

        return errors;
    }
}
