using dinhgallery_api.BusinessObjects;
using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Infrastructures;

public class StorageService : IStorageService
{
    private const string _galleryPath = "/home/www/gallery/";
    private readonly ILogger<StorageService> _logger;
    private readonly FtpClientFactory _ftpClientFactory;
    private readonly StorageSettingsOptions _storageSettings;

    public StorageService(
        FtpClientFactory ftpClientFactory,
        IOptions<StorageSettingsOptions> options,
        ILogger<StorageService> logger)
    {
        _ftpClientFactory = ftpClientFactory;
        _storageSettings = options.Value;
        _logger = logger;
    }

    public async Task<bool> DeleteFileAsync(Uri absoluteUri)
    {
        _logger.LogInformation($"Begin deleting file from storage. File uri: {absoluteUri}.");
        try
        {
            using (var ftpClient = _ftpClientFactory.GetClient())
            {
                await ftpClient.AutoConnectAsync();
                _logger.LogInformation($"Current working directory: {await ftpClient.GetWorkingDirectoryAsync()}");
                await ftpClient.SetWorkingDirectoryAsync(_galleryPath);

                string? filePath = absoluteUri.PathAndQuery.Split("/gallery/").Last();
                _logger.LogInformation($"File path in storage: '{filePath}'.");
                if (await ftpClient.FileExistsAsync(filePath))
                {
                    _logger.LogInformation("File found. Prepare for deletion.");
                    await ftpClient.DeleteFileAsync(filePath);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete file: {absoluteUri.AbsolutePath}. Reason: {ex.Message}.");
            return false;
        }
    }

    public async Task<bool> DeleteFolderAsync(string physicalFolderName)
    {
        _logger.LogInformation($"About to delete folder {physicalFolderName}.");
        try
        {
            using (var ftpClient = _ftpClientFactory.GetClient())
            {
                await ftpClient.AutoConnectAsync();
                await ftpClient.SetWorkingDirectoryAsync(_galleryPath);
                if (await ftpClient.DirectoryExistsAsync(physicalFolderName))
                {
                    _logger.LogInformation($"Folder found.");
                    await ftpClient.DeleteDirectoryAsync(physicalFolderName);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete folder: {physicalFolderName}. Reason: {ex.Message}.");
            return false;
        }
    }

    public async Task<List<GalleryFileAddInput>> SaveAsync(string physicalFolderName, IFormFileCollection formFiles)
    {
        List<GalleryFileAddInput> addedFiles = new();
        using (var ftpClient = _ftpClientFactory.GetClient())
        {
            await ftpClient.AutoConnectAsync();
            await ftpClient.SetWorkingDirectoryAsync(_galleryPath);
            if (!await ftpClient.DirectoryExistsAsync(physicalFolderName))
            {
                await ftpClient.CreateDirectoryAsync(physicalFolderName);
            }
            await ftpClient.SetWorkingDirectoryAsync(_galleryPath + physicalFolderName + "/");
            foreach (IFormFile file in formFiles)
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
                            await ftpClient.UploadStreamAsync(fileStream, physicalFileName);
                            addedFiles.Add(new GalleryFileAddInput
                            {
                                DisplayName = file.FileName,
                                DownloadUri = new Uri($"{_storageSettings.StorageServiceBaseUrl}/gallery/{physicalFolderName}/{physicalFileName}"),
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while saving '{file.FileName}'. Reason: {ex.Message}.");
                        continue;
                    }
                }
            }
        }

        return addedFiles;
    }
}