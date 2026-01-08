using dinhgallery_api.BusinessObjects.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using dinhgallery_api.Infrastructures;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.AddFilesToFolder;

/// <summary>
/// Handler for the AddFilesToFolderCommand.
/// </summary>
public class AddFilesToFolderCommandHandler
    : ICommandHandler<AddFilesToFolderCommand, Ulid?>
{
    private readonly ILogger<AddFilesToFolderCommandHandler> _logger;
    private readonly IGalleryFileWriteRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly IGalleryQueryRepository _queryRepository;
    private readonly IVideoProcessingService _videoProcessingService;

    public AddFilesToFolderCommandHandler(
        ILogger<AddFilesToFolderCommandHandler> logger,
        IGalleryFileWriteRepository fileRepository,
        IGalleryQueryRepository queryRepository,
        IStorageService storageService,
        IVideoProcessingService videoProcessingService)
    {
        _logger = logger;
        _storageService = storageService;
        _fileRepository = fileRepository;
        _queryRepository = queryRepository;
        _videoProcessingService = videoProcessingService;
    }

    public async Task<Ulid?> HandleAsync(
        AddFilesToFolderCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Adding {FileCount} files to folder {FolderId}",
            command.Files.Count,
            command.FolderId);

        // Verify folder exists
        FolderDetailsReadModel? existingFolder = await _queryRepository.GetFolderDetailsAsync(command.FolderId);
        if (existingFolder == null)
        {
            _logger.LogWarning("Folder {FolderId} not found", command.FolderId);
            return null;
        }

        _logger.LogInformation("Existing folder found: {FolderId}", existingFolder.Id);

        string physicalFolderName = existingFolder.PhysicalName;
        Ulid folderId = existingFolder.Id;

        // Write files to storage
        List<GalleryFileAddInput> savedFiles = await _storageService.SaveAsync(physicalFolderName, command.Files);

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
                _logger.LogInformation("Processing video file for streaming optimization: {FileName}", physicalFileName);
                await _videoProcessingService.OptimizeForStreamingAsync(physicalFilePath);
            }
        }

        // Save file records to database
        List<Task<Ulid?>> persistFileTasks = [];
        foreach (GalleryFileAddInput savedFile in savedFiles)
        {
            savedFile.FolderId = folderId;
            persistFileTasks.Add(_fileRepository.AddAsync(savedFile));
        }

        await Task.WhenAll(persistFileTasks);
        return folderId;
    }
}
