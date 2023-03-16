using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;
public interface IGalleryQueryService
{
    Task<List<FolderDetailsReadModel>> GetFolderListAsync();
    Task<FolderDetailsReadModel?> GetFolderDetailsAsync(Ulid folderId);
    Task<FileDetailsReadModel?> GetFileDetailsAsync(Ulid fileId);
}