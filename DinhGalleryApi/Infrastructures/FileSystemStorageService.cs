using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Infrastructures;

public class FileSystemStorageService : IStorageService
{
    private const string _galleryPath = "storage";
    private readonly ILogger<FileSystemStorageService> _logger;
    private readonly StorageSettingsOptions _storageSettings;

    public FileSystemStorageService(
        IOptions<StorageSettingsOptions> options,
        ILogger<FileSystemStorageService> logger)
    {
        _storageSettings = options.Value;
        _logger = logger;
    }

    public Task<bool> DeleteFileAsync(Uri absoluteUri)
    {
        _logger.LogInformation("Begin deleting file from storage. File uri: {FileUri}.", absoluteUri);
        try
        {
            string filePath = Path.GetFileName(absoluteUri.PathAndQuery);
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), _galleryPath, filePath);

            _logger.LogInformation("File path in storage: '{FullPath}'.", fullPath);
            if (File.Exists(fullPath))
            {
                _logger.LogInformation("File found. Prepare for deletion.");
                File.Delete(fullPath);
            }

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FilePath}. Reason: {Reason}.", absoluteUri.AbsolutePath, ex.Message);
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteFolderAsync(string physicalFolderName)
    {
        _logger.LogInformation("About to delete folder {PhysicalFolderName}.", physicalFolderName);
        try
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), _galleryPath, physicalFolderName);

            if (Directory.Exists(fullPath))
            {
                _logger.LogInformation("Folder found: \"{FullPath}\".", fullPath);
                Directory.Delete(fullPath, true);
            }

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete folder: {PhysicalFolderName}. Reason: {Reason}.", physicalFolderName, ex.Message);
            return Task.FromResult(false);
        }
    }

    public async Task<List<GalleryFileAddInput>> SaveAsync(string physicalFolderName, IEnumerable<IFormFile> formFiles)
    {
        List<GalleryFileAddInput> addedFiles = new();
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "storage", physicalFolderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        foreach (IFormFile file in formFiles)
        {
            if (file.Length > 0)
            {
                Guid fileId = Guid.NewGuid();
                string physicalFileName = fileId.ToString() + Path.GetExtension(file.FileName);
                string physicalFilePath = Path.Combine(folderPath, physicalFileName);
                try
                {
                    using (FileStream fileStream = new(physicalFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        addedFiles.Add(new GalleryFileAddInput
                        {
                            DisplayName = file.FileName,
                            DownloadUri = new Uri($"{_storageSettings.StorageServiceBaseUrl}/storage/{physicalFolderName}/{physicalFileName}"),
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while saving '{FileName}'. Reason: {Reason}.", file.FileName, ex.Message);
                    continue;
                }
            }
        }

        return addedFiles;
    }
}