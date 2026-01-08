using dinhgallery_api.BusinessObjects.Commands;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.AddFilesToFolder;

[ApiController]
[Route("gallery")]
public class AddFilesToFolderController : ControllerBase
{
    private readonly ICommandHandler<AddFilesToFolderCommand, Ulid?> _commandHandler;

    public AddFilesToFolderController(ICommandHandler<AddFilesToFolderCommand, Ulid?> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    /// <summary>
    /// Adds files to an existing folder
    /// </summary>
    [HttpPost]
    [Route("folder/{folderId}/files")]
    public async Task<Ulid?> AddFilesToFolder(Ulid folderId, [FromForm] List<IFormFile> files)
    {
        var command = new AddFilesToFolderCommand
        {
            FolderId = folderId,
            Files = files
        };

        return await _commandHandler.HandleAsync(command);
    }
}
