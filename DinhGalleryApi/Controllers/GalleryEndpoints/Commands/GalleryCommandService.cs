using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using dinhgallery_api.Infrastructures;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public class GalleryCommandService : IGalleryCommandService
{
    private readonly ILogger<GalleryCommandService> _logger;
    private readonly IGalleryFolderWriteRepository _folderRepository;
    private readonly IGalleryFileWriteRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly IGalleryQueryRepository _queryRepository;
    private readonly IVideoProcessingService _videoProcessingService;

    public GalleryCommandService(
        ILogger<GalleryCommandService> logger,
        IGalleryFolderWriteRepository folderRepository,
        IGalleryFileWriteRepository fileRepository,
        IGalleryQueryRepository queryRepository,
        IStorageService storageService,
        IVideoProcessingService videoProcessingService)
    {
        _logger = logger;
        _folderRepository = folderRepository;
        _storageService = storageService;
        _fileRepository = fileRepository;
        _queryRepository = queryRepository;
        _videoProcessingService = videoProcessingService;
    }

    public async Task<bool> DeleteFileAsync(Ulid fileId)
    {
        _logger.LogInformation("Begin DeleteFileAsync with fileId: {FileId}.", fileId);
        _logger.LogInformation("Looking for file details in db. fileId: {FileId}.", fileId);
        FileDetailsReadModel? fileDetails = await _queryRepository.GetFileDetailsAsync(fileId);
        if (fileDetails == null)
        {
            _logger.LogInformation("Couldn't find file in db. fileId: {FileId}.", fileId);
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

        Ulid folderId;
        string physicalFolderName;
        FolderDetailsReadModel? existingFolder = input.FolderId.HasValue ? await _queryRepository.GetFolderDetailsAsync(input.FolderId.Value) : null;
        if (existingFolder != null)
        {
            _logger.LogInformation("Existing folder found: {FolderId}.", existingFolder.Id);
            folderId = existingFolder.Id;
            physicalFolderName = existingFolder.PhysicalName;
        }
        else
        {
            // Create a new folder
            physicalFolderName = Ulid.NewUlid().ToString();
            Ulid? newFolderId = await _folderRepository.AddAsync(new()
            {
                DisplayName = input.FolderDisplayName ?? physicalFolderName,
                PhysicalName = physicalFolderName,
            });

            if (!newFolderId.HasValue)
            {
                return null; // Failed to create folder in database
            }

            folderId = newFolderId.Value;
        }

        // Write files to storage
        List<GalleryFileAddInput> savedFiles = await _storageService.SaveAsync(physicalFolderName, input.FormFiles);

        // Optimize videos for streaming (fixes iOS playback issues)
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "storage", physicalFolderName);
        foreach (GalleryFileAddInput savedFile in savedFiles)
        {
            if (savedFile.DownloadUri == null)
            {
                continue;
            }

            // Extract physical file name from download URI
            string physicalFileName = Path.GetFileName(savedFile.DownloadUri.PathAndQuery);
            string physicalFilePath = Path.Combine(folderPath, physicalFileName);

            // Process video files to move moov atom to beginning
            if (_videoProcessingService.IsVideoFile(physicalFilePath))
            {
                _logger.LogInformation("Processing video file for streaming optimization: {FileName}.", physicalFileName);
                await _videoProcessingService.OptimizeForStreamingAsync(physicalFilePath);
            }
        }

        // Save file records to database
        List<Task<Ulid?>> persitFileTasks = [];
        foreach (GalleryFileAddInput savedFile in savedFiles)
        {
            savedFile.FolderId = folderId;
            persitFileTasks.Add(_fileRepository.AddAsync(savedFile));
        }

        await Task.WhenAll(persitFileTasks);
        return folderId;
    }
}