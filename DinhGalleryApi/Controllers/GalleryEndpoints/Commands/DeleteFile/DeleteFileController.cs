using dinhgallery_api.BusinessObjects.Commands;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFile;

[ApiController]
[Route("gallery")]
public class DeleteFileController : ControllerBase
{
    private readonly ICommandHandler<DeleteFileCommand, bool> _commandHandler;

    public DeleteFileController(ICommandHandler<DeleteFileCommand, bool> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    /// <summary>
    /// Deletes a file by its ID
    /// </summary>
    [HttpDelete]
    [Route("file/{id}")]
    public async Task<bool> DeleteFile(Ulid id)
    {
        var command = new DeleteFileCommand
        {
            FileId = id
        };

        return await _commandHandler.HandleAsync(command);
    }
}
