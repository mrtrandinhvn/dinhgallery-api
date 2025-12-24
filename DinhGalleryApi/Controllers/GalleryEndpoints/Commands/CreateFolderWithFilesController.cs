using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class CreateFolderWithFilesController : ControllerBase
{
    private readonly IGalleryCommandService _commandService;

    public CreateFolderWithFilesController(IGalleryCommandService commandService)
    {
        _commandService = commandService;
    }

    /// <summary>
    /// Creates a new folder and uploads files to it
    /// </summary>
    [HttpPost]
    public async Task<Ulid?> CreateFolderWithFiles(
        [FromForm][StringLength(250)] string? folderDisplayName,
        [FromForm] List<IFormFile> files)
    {
        var savedFolderId = await _commandService.SaveFilesAsync(new SaveFilesInput
        {
            FolderDisplayName = folderDisplayName,
            FormFiles = files,
        });

        return savedFolderId;
    }
}
