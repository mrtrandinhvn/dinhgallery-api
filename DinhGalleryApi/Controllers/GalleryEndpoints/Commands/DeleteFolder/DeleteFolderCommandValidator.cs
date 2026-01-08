using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFolder;

/// <summary>
/// Validator for DeleteFolderCommand.
/// </summary>
public class DeleteFolderCommandValidator
    : ICommandValidator<DeleteFolderCommand>
{
    public CommandValidationResult Validate(DeleteFolderCommand command)
    {
        // Ulid validation is handled by type system
        // No additional validation needed
        return CommandValidationResult.Success();
    }
}
