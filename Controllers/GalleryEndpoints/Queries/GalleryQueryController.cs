using dinhgallery_api.BusinessObjects.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

[Authorize(Roles = AppRole.Admin)]
[ApiController]
[Route("gallery")]
public class GalleryQueryController : ControllerBase
{
    private readonly ILogger<GalleryQueryService> _logger;
    private readonly IGalleryQueryService _service;

    public GalleryQueryController(
        ILogger<GalleryQueryService> logger,
        IGalleryQueryService service)
    {
        this._logger = logger;
        _service = service;
    }

    public Task<List<Uri>> Get()
    {
        return _service.GetAllUrisAsync();
    }

    [AllowAnonymous]
    [Route("{filename}")]
    public Uri GetByName(string fileName)
    {
        return _service.GetUriByName(fileName);
    }
}