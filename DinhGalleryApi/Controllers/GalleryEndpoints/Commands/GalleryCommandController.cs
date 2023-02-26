using dinhgallery_api.Controllers.GalleryEndpoints.Queries;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class GalleryCommandControler : ControllerBase
{
    private readonly IGalleryCommandService _commandService;
    private readonly IGalleryQueryService _queryService;

    public GalleryCommandControler(
        IGalleryCommandService commandService,
        IGalleryQueryService queryService)
    {
        _commandService = commandService;
        this._queryService = queryService;
    }

    [HttpDelete]
    [Route("{fileName}")]
    public Task<bool> Delete(string fileId)
    {
        return _commandService.DeleteAsync(fileId);
    }

    [HttpPost]
    public async Task<Guid> Post(string? folderDisplayName)
    {
        var savedFolderId = (await _commandService.SaveFilesAsync(new SaveFilesInput
        {
            FolderDisplayName = folderDisplayName,
            FormFiles = Request.Form.Files,
        }));

        return savedFolderId;
    }
}