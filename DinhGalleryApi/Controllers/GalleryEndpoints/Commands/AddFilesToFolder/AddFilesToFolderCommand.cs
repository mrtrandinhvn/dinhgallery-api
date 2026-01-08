using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.AddFilesToFolder;

/// <summary>
/// Command to add files to an existing folder.
/// </summary>
public class AddFilesToFolderCommand : ICommand<Ulid?>
{
    /// <summary>
    /// The ID of the folder to add files to.
    /// </summary>
    public Ulid FolderId { get; set; }

    /// <summary>
    /// The files to upload.
    /// </summary>
    public List<IFormFile> Files { get; set; } = [];
}
