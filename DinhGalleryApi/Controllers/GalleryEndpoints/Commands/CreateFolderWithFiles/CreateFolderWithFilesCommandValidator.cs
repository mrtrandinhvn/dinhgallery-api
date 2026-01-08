using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.CreateFolderWithFiles;

/// <summary>
/// Validator for CreateFolderWithFilesCommand.
/// </summary>
public class CreateFolderWithFilesCommandValidator
    : ICommandValidator<CreateFolderWithFilesCommand>
{
    public CommandValidationResult Validate(CreateFolderWithFilesCommand command)
    {
        var errors = new List<string>();

        if (command.Files == null || command.Files.Count == 0)
        {
            errors.Add("At least one file must be provided");
        }

        if (!string.IsNullOrWhiteSpace(command.FolderDisplayName) && command.FolderDisplayName.Length > 250)
        {
            errors.Add($"Folder display name exceeds maximum length of 250 characters (actual: {command.FolderDisplayName.Length})");
        }

        return errors.Any()
            ? CommandValidationResult.Failure(errors.ToArray())
            : CommandValidationResult.Success();
    }
}
