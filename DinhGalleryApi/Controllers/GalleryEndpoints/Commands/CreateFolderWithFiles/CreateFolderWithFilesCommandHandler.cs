using dinhgallery_api.BusinessObjects.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Infrastructures;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.CreateFolderWithFiles;

/// <summary>
/// Handler for the CreateFolderWithFilesCommand.
/// </summary>
public class CreateFolderWithFilesCommandHandler
    : ICommandHandler<CreateFolderWithFilesCommand, Ulid?>
{
    private readonly ILogger<CreateFolderWithFilesCommandHandler> _logger;
    private readonly IGalleryFolderWriteRepository _folderRepository;
    private readonly IGalleryFileWriteRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly IVideoProcessingService _videoProcessingService;

    public CreateFolderWithFilesCommandHandler(
        ILogger<CreateFolderWithFilesCommandHandler> logger,
        IGalleryFolderWriteRepository folderRepository,
        IGalleryFileWriteRepository fileRepository,
        IStorageService storageService,
        IVideoProcessingService videoProcessingService)
    {
        _logger = logger;
        _folderRepository = folderRepository;
        _storageService = storageService;
        _fileRepository = fileRepository;
        _videoProcessingService = videoProcessingService;
    }

    public async Task<Ulid?> HandleAsync(
        CreateFolderWithFilesCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating new folder with {FileCount} files. Display name: {DisplayName}",
            command.Files.Count,
            command.FolderDisplayName ?? "(auto-generated)");

        // Create a new folder
        string physicalFolderName = Ulid.NewUlid().ToString();
        Ulid? newFolderId = await _folderRepository.AddAsync(new()
        {
            DisplayName = command.FolderDisplayName ?? physicalFolderName,
            PhysicalName = physicalFolderName,
        });

        if (!newFolderId.HasValue)
        {
            _logger.LogError("Failed to create folder in database");
            return null;
        }

        Ulid folderId = newFolderId.Value;
        _logger.LogInformation("Created folder {FolderId} with physical name {PhysicalName}", folderId, physicalFolderName);

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
