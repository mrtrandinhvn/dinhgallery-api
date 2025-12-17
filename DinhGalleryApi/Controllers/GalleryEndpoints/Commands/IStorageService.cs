using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public interface IStorageService
{
    /// <summary>
    /// Asynchronously saves the specified files to the given physical folder and returns information about the saved
    /// files. Automatically creates the folder if it does not exist.
    /// </summary>
    /// <param name="physicalFolderName">The name of the physical folder where the files will be saved. Cannot be null or empty.</param>
    /// <param name="files">A collection of files to be saved. Each file must be a valid, non-null <see cref="IFormFile"/> instance.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains a list of <see
    /// cref="GalleryFileAddInput"/> objects describing the saved files. The list will be empty if no files are saved.</returns>
    Task<List<GalleryFileAddInput>> SaveAsync(string physicalFolderName, IEnumerable<IFormFile> files);

    Task<bool> DeleteFolderAsync(string physicalFolderName);

    Task<bool> DeleteFileAsync(Uri absoluteUri);
}