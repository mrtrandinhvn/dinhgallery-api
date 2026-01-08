using dinhgallery_api.BusinessObjects.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using dinhgallery_api.Infrastructures;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFile;

/// <summary>
/// Handler for the DeleteFileCommand.
/// </summary>
public class DeleteFileCommandHandler
    : ICommandHandler<DeleteFileCommand, bool>
{
    private readonly ILogger<DeleteFileCommandHandler> _logger;
    private readonly IGalleryFolderWriteRepository _folderRepository;
    private readonly IGalleryFileWriteRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly IGalleryQueryRepository _queryRepository;

    public DeleteFileCommandHandler(
        ILogger<DeleteFileCommandHandler> logger,
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
        DeleteFileCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Begin DeleteFileAsync with fileId: {FileId}", command.FileId);
        _logger.LogInformation("Looking for file details in db. fileId: {FileId}", command.FileId);

        FileDetailsReadModel? fileDetails = await _queryRepository.GetFileDetailsAsync(command.FileId);
        if (fileDetails == null)
        {
            _logger.LogInformation("Couldn't find file in db. fileId: {FileId}", command.FileId);
            return true;
        }

        FolderDetailsReadModel? folderDetails = await _queryRepository.GetFolderDetailsAsync(fileDetails.FolderId);
        if (await _storageService.DeleteFileAsync(fileDetails.DownloadUri!))
        {
            await _fileRepository.DeleteAsync(command.FileId);
        }

        if (folderDetails != null && folderDetails.Files.Count() <= 1)
        {
            // delete the folder if this is the last file
            _logger.LogInformation("Deleting folder {FolderId} as file {FileId} was the last file", folderDetails.Id, command.FileId);
            if (await _storageService.DeleteFolderAsync(folderDetails.PhysicalName))
            {
                await _folderRepository.DeleteAsync(folderDetails.Id);
            }
        }

        return true;
    }
}
