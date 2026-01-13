using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderList;

/// <summary>
/// Query to get a paginated list of folders in the gallery.
/// </summary>
public class GetFolderListQuery : IQuery<PaginatedResult<FolderDetailsReadModel>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
