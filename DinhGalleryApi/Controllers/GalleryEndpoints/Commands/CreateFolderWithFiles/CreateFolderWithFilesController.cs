using System.ComponentModel.DataAnnotations;
using dinhgallery_api.BusinessObjects.Commands;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.CreateFolderWithFiles;

[ApiController]
[Route("gallery")]
public class CreateFolderWithFilesController : ControllerBase
{
    private readonly ICommandHandler<CreateFolderWithFilesCommand, Ulid?> _commandHandler;

    public CreateFolderWithFilesController(ICommandHandler<CreateFolderWithFilesCommand, Ulid?> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    /// <summary>
    /// Creates a new folder and uploads files to it
    /// </summary>
    [HttpPost]
    public async Task<Ulid?> CreateFolderWithFiles(
        [FromForm][StringLength(250)] string? folderDisplayName,
        [FromForm] List<IFormFile> files)
    {
        var command = new CreateFolderWithFilesCommand
        {
            FolderDisplayName = folderDisplayName,
            Files = files
        };

        return await _commandHandler.HandleAsync(command);
    }
}
