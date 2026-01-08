using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderList;

/// <summary>
/// Query to get a list of all folders in the gallery.
/// </summary>
public class GetFolderListQuery : IQuery<List<FolderDetailsReadModel>>
{
}
