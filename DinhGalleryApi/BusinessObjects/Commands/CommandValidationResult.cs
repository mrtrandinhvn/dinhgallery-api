namespace dinhgallery_api.BusinessObjects.Commands;

/// <summary>
/// Represents the result of command validation.
/// </summary>
public class CommandValidationResult
{
    /// <summary>
    /// Gets whether the validation was successful.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the list of validation error messages.
    /// </summary>
    public List<string> Errors { get; init; } = new();

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static CommandValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed validation result with error messages.
    /// </summary>
    public static CommandValidationResult Failure(params string[] errors) =>
        new() { IsValid = false, Errors = errors.ToList() };
}
