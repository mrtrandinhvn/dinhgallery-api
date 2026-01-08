using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderList;

/// <summary>
/// Handler for the GetFolderListQuery.
/// </summary>
public class GetFolderListQueryHandler
    : IQueryHandler<GetFolderListQuery, List<FolderDetailsReadModel>>
{
    private readonly ILogger<GetFolderListQueryHandler> _logger;
    private readonly IGalleryQueryRepository _repository;

    public GetFolderListQueryHandler(
        ILogger<GetFolderListQueryHandler> logger,
        IGalleryQueryRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<List<FolderDetailsReadModel>> HandleAsync(
        GetFolderListQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving folder list");

        return await _repository.GetFolderListAsync();
    }
}
