namespace dinhgallery_api.BusinessObjects.Commands;

/// <summary>
/// Defines a validator for a command.
/// Validators contain the validation rules for a specific command type.
/// </summary>
/// <typeparam name="TCommand">The type of command to validate</typeparam>
public interface ICommandValidator<in TCommand>
{
    /// <summary>
    /// Validates the specified command.
    /// </summary>
    /// <param name="command">The command to validate</param>
    /// <returns>A CommandValidationResult indicating success or failure with error messages</returns>
    CommandValidationResult Validate(TCommand command);
}
