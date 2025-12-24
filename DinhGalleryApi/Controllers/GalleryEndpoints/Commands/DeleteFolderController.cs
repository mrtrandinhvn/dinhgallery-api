using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class DeleteFolderController : ControllerBase
{
    private readonly IGalleryCommandService _commandService;

    public DeleteFolderController(IGalleryCommandService commandService)
    {
        _commandService = commandService;
    }

    /// <summary>
    /// Deletes a folder and all its files by folder ID
    /// </summary>
    [HttpDelete]
    [Route("folder/{id}")]
    public Task<bool> DeleteFolder(Ulid id)
    {
        return _commandService.DeleteFolderAsync(id);
    }
}
