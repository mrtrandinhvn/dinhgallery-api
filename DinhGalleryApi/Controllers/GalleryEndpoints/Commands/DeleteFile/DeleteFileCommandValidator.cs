using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFile;

/// <summary>
/// Validator for DeleteFileCommand.
/// </summary>
public class DeleteFileCommandValidator
    : ICommandValidator<DeleteFileCommand>
{
    public CommandValidationResult Validate(DeleteFileCommand command)
    {
        // Ulid validation is handled by type system
        // No additional validation needed
        return CommandValidationResult.Success();
    }
}
