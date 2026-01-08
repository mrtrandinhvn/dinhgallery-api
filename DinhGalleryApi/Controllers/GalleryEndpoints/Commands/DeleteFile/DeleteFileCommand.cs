using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFile;

/// <summary>
/// Command to delete a file by its ID.
/// Also deletes the parent folder if this is the last file in the folder.
/// </summary>
public class DeleteFileCommand : ICommand<bool>
{
    /// <summary>
    /// The ID of the file to delete.
    /// </summary>
    public Ulid FileId { get; set; }
}
