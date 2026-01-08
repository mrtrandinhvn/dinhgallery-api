using dinhgallery_api.BusinessObjects.Commands;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.UpdateFolderDisplayName;

[ApiController]
[Route("gallery")]
public class UpdateFolderDisplayNameController : ControllerBase
{
    private readonly ICommandHandler<UpdateFolderDisplayNameCommand, bool> _handler;

    public UpdateFolderDisplayNameController(
        ICommandHandler<UpdateFolderDisplayNameCommand, bool> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Updates the display name of a folder
    /// </summary>
    [HttpPatch]
    [Route("folder/{id}/display-name")]
    public Task<bool> UpdateFolderDisplayName(Ulid id, [FromBody] UpdateFolderDisplayNameRequest request)
    {
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = id,
            DisplayName = request.DisplayName
        };

        return _handler.HandleAsync(command);
    }
}

public class UpdateFolderDisplayNameRequest
{
    [Required(ErrorMessage = "Display name is required")]
    [StringLength(250, MinimumLength = 1, ErrorMessage = "Display name must be between 1 and 250 characters")]
    public string DisplayName { get; set; } = string.Empty;
}
