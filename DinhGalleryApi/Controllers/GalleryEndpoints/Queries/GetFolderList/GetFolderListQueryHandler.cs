using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderList;

/// <summary>
/// Handler for the GetFolderListQuery.
/// </summary>
public class GetFolderListQueryHandler
    : IQueryHandler<GetFolderListQuery, PaginatedResult<FolderDetailsReadModel>>
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

    public async Task<PaginatedResult<FolderDetailsReadModel>> HandleAsync(
        GetFolderListQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving folder list (Page {PageNumber}, Size {PageSize})",
            query.PageNumber, query.PageSize);

        var skip = (query.PageNumber - 1) * query.PageSize;
        var (items, totalCount) = await _repository.GetFolderListAsync(skip, query.PageSize);

        return new PaginatedResult<FolderDetailsReadModel>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}
