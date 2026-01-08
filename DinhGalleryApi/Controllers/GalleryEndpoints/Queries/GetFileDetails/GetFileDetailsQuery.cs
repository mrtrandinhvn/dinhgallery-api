using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFileDetails;

/// <summary>
/// Query to get details of a specific file by ID.
/// </summary>
public class GetFileDetailsQuery : IQuery<FileDetailsResponse?>
{
    /// <summary>
    /// The ID of the file to retrieve.
    /// </summary>
    public Ulid FileId { get; set; }
}
