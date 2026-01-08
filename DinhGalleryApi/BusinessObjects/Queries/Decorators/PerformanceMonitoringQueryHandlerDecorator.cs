using System.Diagnostics;

namespace dinhgallery_api.BusinessObjects.Queries.Decorators;

/// <summary>
/// Decorator that adds performance monitoring to query handler execution.
/// Measures execution time and logs performance metrics.
/// </summary>
public class PerformanceMonitoringQueryHandlerDecorator<TQuery, TResult>
    : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _innerHandler;
    private readonly ILogger<PerformanceMonitoringQueryHandlerDecorator<TQuery, TResult>> _logger;

    public PerformanceMonitoringQueryHandlerDecorator(
        IQueryHandler<TQuery, TResult> innerHandler,
        ILogger<PerformanceMonitoringQueryHandlerDecorator<TQuery, TResult>> logger)
    {
        _innerHandler = innerHandler;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var queryName = typeof(TQuery).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Executing query {QueryName}",
            queryName);

        try
        {
            var result = await _innerHandler.HandleAsync(query, cancellationToken);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "Query {QueryName} completed successfully in {ElapsedMs}ms",
                queryName,
                elapsedMs);

            // Log warning for slow operations
            if (elapsedMs > 1000) // TODO: Make threshold configurable
            {
                _logger.LogWarning(
                    "Slow query detected: {QueryName} took {ElapsedMs}ms",
                    queryName,
                    elapsedMs);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Query {QueryName} failed after {ElapsedMs}ms",
                queryName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
