using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.UpdateFolderDisplayName;

/// <summary>
/// Command to update a folder's display name.
/// </summary>
public class UpdateFolderDisplayNameCommand : ICommand<bool>
{
    /// <summary>
    /// The ID of the folder to update.
    /// </summary>
    public Ulid FolderId { get; set; }

    /// <summary>
    /// The new display name for the folder.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}
