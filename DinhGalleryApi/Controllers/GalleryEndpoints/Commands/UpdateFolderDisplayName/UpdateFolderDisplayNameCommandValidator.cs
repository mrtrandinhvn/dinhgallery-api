using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.UpdateFolderDisplayName;

/// <summary>
/// Validator for UpdateFolderDisplayNameCommand.
/// </summary>
public class UpdateFolderDisplayNameCommandValidator
    : ICommandValidator<UpdateFolderDisplayNameCommand>
{
    public CommandValidationResult Validate(UpdateFolderDisplayNameCommand command)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(command.DisplayName))
        {
            errors.Add("Display name cannot be empty");
        }

        if (command.DisplayName?.Length > 250)
        {
            errors.Add($"Display name exceeds maximum length of 250 characters (actual: {command.DisplayName.Length})");
        }

        return errors.Any()
            ? CommandValidationResult.Failure(errors.ToArray())
            : CommandValidationResult.Success();
    }
}
