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
    public Task<List<Uri>> Get()
    {
        return _service.GetAllUrisAsync();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{fileName}")]
    public Uri GetByName(string fileName)
    {
        return _service.GetUriByName(fileName);
    }
}