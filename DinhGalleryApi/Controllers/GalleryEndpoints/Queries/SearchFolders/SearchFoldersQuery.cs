using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.SearchFolders;

public class SearchFoldersQuery : IQuery<List<FolderDetailsReadModel>>
{
    public required string SearchText { get; init; }
}
