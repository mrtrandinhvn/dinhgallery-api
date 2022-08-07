using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class GalleryCommandControler : ControllerBase
{
    private readonly IGalleryCommandService _service;

    public GalleryCommandControler(IGalleryCommandService service)
    {
        _service = service;
    }

    [HttpDelete]
    [Route("{fileName}")]
    public Task<bool> Delete(string fileName)
    {
        return _service.DeleteAsync(fileName);
    }
}