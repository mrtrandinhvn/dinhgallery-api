using dinhgallery_api.BusinessObjects.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.UpdateFolderDisplayName;

/// <summary>
/// Handler for the UpdateFolderDisplayNameCommand.
/// </summary>
public class UpdateFolderDisplayNameCommandHandler
    : ICommandHandler<UpdateFolderDisplayNameCommand, bool>
{
    private readonly ILogger<UpdateFolderDisplayNameCommandHandler> _logger;
    private readonly IGalleryFolderWriteRepository _folderRepository;

    public UpdateFolderDisplayNameCommandHandler(
        ILogger<UpdateFolderDisplayNameCommandHandler> logger,
        IGalleryFolderWriteRepository folderRepository)
    {
        _logger = logger;
        _folderRepository = folderRepository;
    }

    public async Task<bool> HandleAsync(
        UpdateFolderDisplayNameCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Updating folder {FolderId} display name to {DisplayName}",
            command.FolderId,
            command.DisplayName);

        UpdateFolderDisplayNameInput input = new()
        {
            FolderId = command.FolderId,
            DisplayName = command.DisplayName
        };

        return await _folderRepository.UpdateAsync(input);
    }
}
