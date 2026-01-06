using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

[ApiController]
[Route("gallery")]
public class UpdateFolderDisplayNameController : ControllerBase
{
    private readonly IGalleryCommandService _commandService;

    public UpdateFolderDisplayNameController(IGalleryCommandService commandService)
    {
        _commandService = commandService;
    }

    /// <summary>
    /// Updates the display name of a folder
    /// </summary>
    [HttpPatch]
    [Route("folder/{id}/display-name")]
    public Task<bool> UpdateFolderDisplayName(Ulid id, [FromBody] UpdateFolderDisplayNameRequest request)
    {
        return _commandService.UpdateFolderDisplayNameAsync(id, request.DisplayName);
    }
}

public class UpdateFolderDisplayNameRequest
{
    [Required(ErrorMessage = "Display name is required")]
    [StringLength(250, MinimumLength = 1, ErrorMessage = "Display name must be between 1 and 250 characters")]
    public string DisplayName { get; set; } = string.Empty;
}
