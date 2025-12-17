using System.ComponentModel.DataAnnotations;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class GalleryCommandController : ControllerBase
{
    private readonly IGalleryCommandService _commandService;
    private readonly IGalleryQueryService _queryService;

    public GalleryCommandController(
        IGalleryCommandService commandService,
        IGalleryQueryService queryService)
    {
        _commandService = commandService;
        this._queryService = queryService;
    }

    [HttpDelete]
    [Route("file/{id}")]
    public Task<bool> DeleteFile(Ulid id)
    {
        return _commandService.DeleteFileAsync(id);
    }

    [HttpDelete]
    [Route("folder/{id}")]
    public Task<bool> DeleteFolder(Ulid id)
    {
        return _commandService.DeleteFolderAsync(id);
    }

    [HttpPost]
    public async Task<Ulid?> Post([FromForm][StringLength(250)] string? folderDisplayName, [FromForm] List<IFormFile> files)
    {
        var savedFolderId = await _commandService.SaveFilesAsync(new SaveFilesInput
        {
            FolderDisplayName = folderDisplayName,
            FormFiles = files,
        });

        return savedFolderId;
    }

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