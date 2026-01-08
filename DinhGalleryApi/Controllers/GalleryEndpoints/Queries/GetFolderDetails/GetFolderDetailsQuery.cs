using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderDetails;

/// <summary>
/// Query to get details of a specific folder by ID.
/// </summary>
public class GetFolderDetailsQuery : IQuery<FolderDetailsReadModel?>
{
    /// <summary>
    /// The ID of the folder to retrieve.
    /// </summary>
    public Ulid FolderId { get; set; }
}
