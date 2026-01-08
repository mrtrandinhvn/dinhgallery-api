namespace dinhgallery_api.BusinessObjects.Commands.Decorators;

/// <summary>
/// Decorator that adds validation to command handler execution.
/// Validates the command before delegating to the inner handler.
/// </summary>
public class ValidationCommandHandlerDecorator<TCommand, TResult>
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _innerHandler;
    private readonly ICommandValidator<TCommand> _validator;
    private readonly ILogger<ValidationCommandHandlerDecorator<TCommand, TResult>> _logger;

    public ValidationCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> innerHandler,
        ICommandValidator<TCommand> validator,
        ILogger<ValidationCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _innerHandler = innerHandler;
        _validator = validator;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(
                "Validation failed for {CommandName}: {Errors}",
                typeof(TCommand).Name,
                string.Join(", ", validationResult.Errors));

            // For bool return type, return false
            // In future, could throw ValidationException or return Result<T>
            return (TResult)(object)false;
        }

        return await _innerHandler.HandleAsync(command, cancellationToken);
    }
}
