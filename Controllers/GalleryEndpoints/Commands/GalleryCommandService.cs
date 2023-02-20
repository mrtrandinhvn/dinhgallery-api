using dinhgallery_api.BusinessObjects;
using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public class GalleryCommandService : IGalleryCommandService
{
    private const string _galleryPath = "/home/www/gallery/";
    private readonly FtpClientFactory _ftpClientFactory;
    private readonly IGalleryFolderRepository _folderRepository;
    private readonly IGalleryFileRepository _fileRepository;
    private readonly StorageSettingsOptions _storageSettings;
    private readonly ILogger<GalleryCommandService> _logger;

    public GalleryCommandService(
        ILogger<GalleryCommandService> logger,
        FtpClientFactory ftpClientFactory,
        IGalleryFolderRepository folderRepository,
        IGalleryFileRepository fileRepository,
        IOptions<StorageSettingsOptions> storageSettingsOptions)
    {
        _logger = logger;
        _ftpClientFactory = ftpClientFactory;
        _folderRepository = folderRepository;
        _fileRepository = fileRepository;
        _storageSettings = storageSettingsOptions.Value;
    }

    public async Task<bool> DeleteAsync(string fileId)
    {
        using (var ftpClient = _ftpClientFactory.GetClient())
        {
            await ftpClient.AutoConnectAsync();
            await ftpClient.SetWorkingDirectoryAsync(_galleryPath);
            if (await ftpClient.FileExistsAsync(fileId))
            {
                await ftpClient.DeleteFileAsync(fileId);
            }

            return true;
        }
    }

    public async Task<Guid> SaveFilesAsync(SaveFilesInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(input.FormFiles);
        Guid folderId = Guid.NewGuid();

        using (var ftpClient = _ftpClientFactory.GetClient())
        {
            await ftpClient.AutoConnectAsync();
            await ftpClient.SetWorkingDirectoryAsync(_galleryPath);
            await ftpClient.CreateDirectoryAsync(folderId.ToString());
            await ftpClient.SetWorkingDirectoryAsync(_galleryPath + folderId + "/");
            foreach (IFormFile file in input.FormFiles)
            {
                if (file.Length > 0)
                {
                    Guid fileId = Guid.NewGuid();
                    string physicalFileName = fileId.ToString() + Path.GetExtension(file.FileName);
                    try
                    {
                        using (MemoryStream fileStream = new())
                        {
                            await file.CopyToAsync(fileStream);
                            var saveToStorageTask = ftpClient.UploadBytesAsync(fileStream.ToArray(), physicalFileName);
                            var saveToDbTask = _fileRepository.AddAsync(new GalleryFileAddInput
                            {
                                Id = fileId,
                                FolderId = folderId,
                                DisplayName = file.FileName,
                                DownloadUrl = new Uri($"{_storageSettings.StorageServiceBaseUrl}/gallery/{folderId}/{physicalFileName}"),
                            });
                            await Task.WhenAll(saveToStorageTask, saveToDbTask);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while saving '{file.FileName}'.");
                        continue;
                    }
                }
            }
        }

        await _folderRepository.AddAsync(new GalleryFolderAddInput
        {
            Id = folderId,
            DisplayName = input.FolderDisplayName ?? folderId.ToString(),
        });

        return folderId;
    }
}