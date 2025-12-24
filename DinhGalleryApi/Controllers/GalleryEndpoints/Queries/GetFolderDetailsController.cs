using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

[ApiController]
[Route("gallery")]
public class GetFolderDetailsController : ControllerBase
{
    private readonly IGalleryQueryRepository _service;

    public GetFolderDetailsController(IGalleryQueryRepository service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets details of a specific folder by ID
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [Route("folder/{id}")]
    public Task<FolderDetailsReadModel?> GetFolderDetails(Ulid id)
    {
        return _service.GetFolderDetailsAsync(id);
    }
}
