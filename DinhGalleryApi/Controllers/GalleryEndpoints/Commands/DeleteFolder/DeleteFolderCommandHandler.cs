using dinhgallery_api.BusinessObjects.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using dinhgallery_api.Infrastructures;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFolder;

/// <summary>
/// Handler for the DeleteFolderCommand.
/// </summary>
public class DeleteFolderCommandHandler
    : ICommandHandler<DeleteFolderCommand, bool>
{
    private readonly ILogger<DeleteFolderCommandHandler> _logger;
    private readonly IGalleryFolderWriteRepository _folderRepository;
    private readonly IGalleryFileWriteRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly IGalleryQueryRepository _queryRepository;

    public DeleteFolderCommandHandler(
        ILogger<DeleteFolderCommandHandler> logger,
        IGalleryFolderWriteRepository folderRepository,
        IGalleryFileWriteRepository fileRepository,
        IGalleryQueryRepository queryRepository,
        IStorageService storageService)
    {
        _logger = logger;
        _folderRepository = folderRepository;
        _storageService = storageService;
        _fileRepository = fileRepository;
        _queryRepository = queryRepository;
    }

    public async Task<bool> HandleAsync(
        DeleteFolderCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting folder {FolderId}", command.FolderId);

        FolderDetailsReadModel? folder = await _queryRepository.GetFolderDetailsAsync(command.FolderId);
        if (folder == null)
        {
            _logger.LogInformation("Folder {FolderId} not found", command.FolderId);
            return true;
        }

        if (!await _storageService.DeleteFolderAsync(folder.PhysicalName))
        {
            _logger.LogError("Failed to delete folder {FolderId} from storage", command.FolderId);
            return false;
        }

        List<Task<bool>> deleteDbTasks = new();
        foreach (FileDetailsReadModel file in folder.Files)
        {
            deleteDbTasks.Add(_fileRepository.DeleteAsync(file.Id));
        }
        deleteDbTasks.Add(_folderRepository.DeleteAsync(command.FolderId));

        bool[] deleteResults = await Task.WhenAll(deleteDbTasks);
        return deleteResults.All(x => x);
    }
}
