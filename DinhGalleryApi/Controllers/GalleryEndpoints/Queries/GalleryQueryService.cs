using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

public class GalleryQueryService : IGalleryQueryService
{
    private readonly IGalleryQueryRepository _queryRepository;

    public GalleryQueryService(IGalleryQueryRepository queryRepository)
    {
        this._queryRepository = queryRepository;
    }

    public Task<FileDetailsReadModel?> GetFileDetailsAsync(Ulid fileId)
    {
        return _queryRepository.GetFileDetailsAsync(fileId);
    }

    public Task<FolderDetailsReadModel?> GetFolderDetailsAsync(Ulid folderId)
    {
        return _queryRepository.GetFolderDetailsAsync(folderId);
    }

    public Task<List<FolderDetailsReadModel>> GetFolderListAsync()
    {
        return _queryRepository.GetFolderListAsync();
    }
}