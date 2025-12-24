using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class DeleteFileController : ControllerBase
{
    private readonly IGalleryCommandService _commandService;

    public DeleteFileController(IGalleryCommandService commandService)
    {
        _commandService = commandService;
    }

    /// <summary>
    /// Deletes a file by its ID
    /// </summary>
    [HttpDelete]
    [Route("file/{id}")]
    public Task<bool> DeleteFile(Ulid id)
    {
        return _commandService.DeleteFileAsync(id);
    }
}
