using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

public interface IGalleryQueryRepository
{
    Task<FileDetailsReadModel?> GetFileDetailsAsync(Ulid fileId);
    Task<FolderDetailsReadModel?> GetFolderDetailsAsync(Ulid folderId);
    Task<List<FolderDetailsReadModel>> GetFolderListAsync();
}