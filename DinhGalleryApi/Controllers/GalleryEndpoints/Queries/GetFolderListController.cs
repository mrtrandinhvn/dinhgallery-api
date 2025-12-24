using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

[ApiController]
[Route("gallery")]
public class GetFolderListController : ControllerBase
{
    private readonly IGalleryQueryRepository _service;

    public GetFolderListController(IGalleryQueryRepository service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets a list of all folders in the gallery
    /// </summary>
    [HttpGet]
    public Task<List<FolderDetailsReadModel>> GetFolderList()
    {
        return _service.GetFolderListAsync();
    }
}
