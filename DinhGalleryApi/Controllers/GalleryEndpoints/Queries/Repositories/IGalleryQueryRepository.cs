using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

public interface IGalleryQueryRepository
{
    Task<FileDetailsReadModel?> GetFileDetailsAsync(Guid fileId);
    Task<FolderDetailsReadModel?> GetFolderDetailsAsync(Guid folderId);
    Task<List<Guid>> GetFolderListAsync();
}