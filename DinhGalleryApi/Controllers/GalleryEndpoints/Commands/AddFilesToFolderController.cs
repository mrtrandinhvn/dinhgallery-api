using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class AddFilesToFolderController : ControllerBase
{
    private readonly IGalleryCommandService _commandService;

    public AddFilesToFolderController(IGalleryCommandService commandService)
    {
        _commandService = commandService;
    }

    /// <summary>
    /// Adds files to an existing folder
    /// </summary>
    [HttpPost]
    [Route("folder/{folderId}/files")]
    public async Task<Ulid?> AddFilesToFolder(Ulid folderId, [FromForm] List<IFormFile> files)
    {
        var savedFolderId = await _commandService.SaveFilesAsync(new SaveFilesInput
        {
            FolderId = folderId,
            FormFiles = files,
        });

        return savedFolderId;
    }
}
