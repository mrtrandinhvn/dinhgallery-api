using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderDetails;

/// <summary>
/// Handler for the GetFolderDetailsQuery.
/// </summary>
public class GetFolderDetailsQueryHandler
    : IQueryHandler<GetFolderDetailsQuery, FolderDetailsReadModel?>
{
    private readonly ILogger<GetFolderDetailsQueryHandler> _logger;
    private readonly IGalleryQueryRepository _repository;

    public GetFolderDetailsQueryHandler(
        ILogger<GetFolderDetailsQueryHandler> logger,
        IGalleryQueryRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<FolderDetailsReadModel?> HandleAsync(
        GetFolderDetailsQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving folder details for {FolderId}",
            query.FolderId);

        return await _repository.GetFolderDetailsAsync(query.FolderId);
    }
}
