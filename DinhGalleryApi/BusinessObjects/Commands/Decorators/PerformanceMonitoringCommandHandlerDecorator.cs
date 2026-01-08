using System.Diagnostics;

namespace dinhgallery_api.BusinessObjects.Commands.Decorators;

/// <summary>
/// Decorator that adds performance monitoring to command handler execution.
/// Measures execution time and logs performance metrics.
/// </summary>
public class PerformanceMonitoringCommandHandlerDecorator<TCommand, TResult>
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _innerHandler;
    private readonly ILogger<PerformanceMonitoringCommandHandlerDecorator<TCommand, TResult>> _logger;

    public PerformanceMonitoringCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> innerHandler,
        ILogger<PerformanceMonitoringCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _innerHandler = innerHandler;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Executing command {CommandName}",
            commandName);

        try
        {
            var result = await _innerHandler.HandleAsync(command, cancellationToken);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "Command {CommandName} completed successfully in {ElapsedMs}ms",
                commandName,
                elapsedMs);

            // Log warning for slow operations
            if (elapsedMs > 1000) // TODO: Make threshold configurable
            {
                _logger.LogWarning(
                    "Slow command detected: {CommandName} took {ElapsedMs}ms",
                    commandName,
                    elapsedMs);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Command {CommandName} failed after {ElapsedMs}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
