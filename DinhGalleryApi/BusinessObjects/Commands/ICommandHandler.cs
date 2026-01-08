namespace dinhgallery_api.BusinessObjects.Commands;

/// <summary>
/// Defines a handler for a command.
/// Each command should have exactly one handler that contains the business logic for executing that command.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle</typeparam>
/// <typeparam name="TResult">The type of result returned by the handler</typeparam>
public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the specified command.
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of executing the command</returns>
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
