using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

[ApiController]
[Route("gallery")]
public class GalleryQueryController : ControllerBase
{
    private readonly IGalleryQueryService _service;

    public GalleryQueryController(IGalleryQueryService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route("folder")]
    public Task<List<Guid>> GetFolderList()
    {
        return _service.GetFolderListAsync();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("folder/{id}")]
    public Task<FolderDetailsReadModel> GetFolderDetails(Guid id)
    {
        return _service.GetFolderDetailsAsync(id);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("file/{id}")]
    public Task<FileDetailsReadModel> GetFileDetails(Guid id)
    {
        return _service.GetFileDetailsAsync(id);
    }
}