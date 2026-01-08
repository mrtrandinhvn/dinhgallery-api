using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderDetails;

[ApiController]
[Route("gallery")]
public class GetFolderDetailsController : ControllerBase
{
    private readonly IQueryHandler<GetFolderDetailsQuery, FolderDetailsReadModel?> _handler;

    public GetFolderDetailsController(
        IQueryHandler<GetFolderDetailsQuery, FolderDetailsReadModel?> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Gets details of a specific folder by ID
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [Route("folder/{id}")]
    public Task<FolderDetailsReadModel?> GetFolderDetails(Ulid id)
    {
        var query = new GetFolderDetailsQuery
        {
            FolderId = id
        };

        return _handler.HandleAsync(query);
    }
}
