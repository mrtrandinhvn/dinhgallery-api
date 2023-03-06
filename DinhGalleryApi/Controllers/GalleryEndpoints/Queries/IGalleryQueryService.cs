using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;
public interface IGalleryQueryService
{
    Task<List<Guid>> GetFolderListAsync();
    Task<FolderDetailsReadModel?> GetFolderDetailsAsync(Guid folderId);
    Task<FileDetailsReadModel?> GetFileDetailsAsync(Guid fileId);
}