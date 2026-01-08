using dinhgallery_api.BusinessObjects.Commands;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFolder;

[ApiController]
[Route("gallery")]
public class DeleteFolderController : ControllerBase
{
    private readonly ICommandHandler<DeleteFolderCommand, bool> _commandHandler;

    public DeleteFolderController(ICommandHandler<DeleteFolderCommand, bool> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    /// <summary>
    /// Deletes a folder and all its files by folder ID
    /// </summary>
    [HttpDelete]
    [Route("folder/{id}")]
    public async Task<bool> DeleteFolder(Ulid id)
    {
        var command = new DeleteFolderCommand
        {
            FolderId = id
        };

        return await _commandHandler.HandleAsync(command);
    }
}
