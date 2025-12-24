using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;
public interface IGalleryQueryService
{
    /// <summary>
    /// Asynchronously retrieves a list of folders with their details.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
    /// cref="FolderDetailsReadModel"/> objects, each representing the details of a folder. The list is empty if no
    /// folders are found.</returns>
    Task<List<FolderDetailsReadModel>> GetFolderListAsync();

    /// <summary>
    /// Asynchronously retrieves the details of the specified folder.   
    /// </summary>
    /// <param name="folderId">The unique identifier of the folder to retrieve details for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="FolderDetailsReadModel"/> with the folder details if found; otherwise, <see langword="null"/>.</returns>
    Task<FolderDetailsReadModel?> GetFolderDetailsAsync(Ulid folderId);

    /// <summary>
    /// Asynchronously retrieves the details of a file identified by the specified unique identifier.   
    /// </summary>
    /// <param name="fileId">The unique identifier of the file to retrieve details for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="FileDetailsReadModel"/>
    /// with the file details if found; otherwise, <see langword="null"/>.</returns>
    Task<FileDetailsReadModel?> GetFileDetailsAsync(Ulid fileId);
}