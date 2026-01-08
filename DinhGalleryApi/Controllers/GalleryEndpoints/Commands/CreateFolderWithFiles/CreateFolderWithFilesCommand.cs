using dinhgallery_api.BusinessObjects.Commands;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.CreateFolderWithFiles;

/// <summary>
/// Command to create a new folder and upload files to it.
/// </summary>
public class CreateFolderWithFilesCommand : ICommand<Ulid?>
{
    /// <summary>
    /// Optional display name for the new folder.
    /// If not provided, the physical folder name will be used.
    /// </summary>
    public string? FolderDisplayName { get; set; }

    /// <summary>
    /// The files to upload to the new folder.
    /// </summary>
    public List<IFormFile> Files { get; set; } = [];
}
