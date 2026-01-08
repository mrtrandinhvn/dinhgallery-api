using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.AddFilesToFolder;

/// <summary>
/// Validator for AddFilesToFolderCommand.
/// </summary>
public class AddFilesToFolderCommandValidator
    : ICommandValidator<AddFilesToFolderCommand>
{
    public CommandValidationResult Validate(AddFilesToFolderCommand command)
    {
        var errors = new List<string>();

        if (command.Files == null || command.Files.Count == 0)
        {
            errors.Add("At least one file must be provided");
        }

        return errors.Any()
            ? CommandValidationResult.Failure(errors.ToArray())
            : CommandValidationResult.Success();
    }
}
