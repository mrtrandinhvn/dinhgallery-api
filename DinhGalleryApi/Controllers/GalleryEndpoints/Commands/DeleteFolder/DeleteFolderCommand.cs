using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFolder;

/// <summary>
/// Command to delete a folder and all its files.
/// </summary>
public class DeleteFolderCommand : ICommand<bool>
{
    /// <summary>
    /// The ID of the folder to delete.
    /// </summary>
    public Ulid FolderId { get; set; }
}
