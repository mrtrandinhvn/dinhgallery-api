using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public class GalleryCommandService : IGalleryCommandService
{
    private readonly ILogger<GalleryCommandService> _logger;
    private readonly IGalleryFolderWriteRepository _folderRepository;
    private readonly IGalleryFileWriteRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly IGalleryQueryRepository _queryRepository;

    public GalleryCommandService(
        ILogger<GalleryCommandService> logger,
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

    public async Task<bool> DeleteFileAsync(Ulid fileId)
    {
        _logger.LogInformation($"Begin DeleteFileAsync with fileId: {fileId}.");
        _logger.LogInformation($"Looking for file details in db. fileId: {fileId}.");
        FileDetailsReadModel? fileDetails = await _queryRepository.GetFileDetailsAsync(fileId);
        if (fileDetails == null)
        {
            _logger.LogInformation($"Couldn't find file in db. fileId: {fileId}.");
            return true;
        }

        FolderDetailsReadModel? folderDetails = await _queryRepository.GetFolderDetailsAsync(fileDetails.FolderId);
        if (await _storageService.DeleteFileAsync(fileDetails.DownloadUri!))
        {
            await _fileRepository.DeleteAsync(fileId);
        }

        if (folderDetails != null && folderDetails.Files.Count() <= 1)
        {
            // delete the folder if this is the last file
            if (await _storageService.DeleteFolderAsync(folderDetails.PhysicalName))
            {
                await _folderRepository.DeleteAsync(folderDetails.Id);
            }
        }

        return true;
    }

    public async Task<bool> DeleteFolderAsync(Ulid folderId)
    {
        FolderDetailsReadModel? folder = await _queryRepository.GetFolderDetailsAsync(folderId);
        if (folder == null)
        {
            return true;
        }

        if (!await _storageService.DeleteFolderAsync(folder.PhysicalName))
        {
            return false;
        }

        List<Task<bool>> deleteDbTasks = new();
        foreach (FileDetailsReadModel file in folder.Files)
        {
            deleteDbTasks.Add(_fileRepository.DeleteAsync(file.Id));
        }
        deleteDbTasks.Add(_folderRepository.DeleteAsync(folderId));

        bool[] deleteResults = await Task.WhenAll(deleteDbTasks);
        return deleteResults.All(x => x);
    }

    public async Task<Ulid?> SaveFilesAsync(SaveFilesInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(input.FormFiles);
        string physicalFolderName = Guid.NewGuid().ToString();
        GalleryFolderAddInput folder = new GalleryFolderAddInput
        {
            DisplayName = input.FolderDisplayName ?? physicalFolderName,
            PhysicalName = physicalFolderName,
        };

        Ulid? folderId = await _folderRepository.AddAsync(folder);
        if (folderId.HasValue)
        {
            List<GalleryFileAddInput> savedFiles = await _storageService.SaveAsync(physicalFolderName, input.FormFiles);
            List<Task<Ulid?>> saveFileTasks = new();
            foreach (GalleryFileAddInput savedFile in savedFiles)
            {
                savedFile.FolderId = folderId.Value;
                saveFileTasks.Add(_fileRepository.AddAsync(savedFile));
            }

            await Task.WhenAll(saveFileTasks);
            return folderId.Value;
        }

        // clean up storage if failed to save record to db
        await _storageService.DeleteFolderAsync(folder.PhysicalName);
        return null;
    }
}