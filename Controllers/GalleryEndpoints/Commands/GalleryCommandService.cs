using dinhgallery_api.BusinessObjects;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public class GalleryCommandService : IGalleryCommandService
{
    private const string _galleryPath = "/home/www/gallery/";
    private readonly FtpClientFactory _ftpClientFactory;

    private readonly ILogger<GalleryCommandService> _logger;

    public GalleryCommandService(
        ILogger<GalleryCommandService> logger,
        FtpClientFactory ftpClientFactory)
    {
        _logger = logger;
        _ftpClientFactory = ftpClientFactory;
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        using (var ftpClient = _ftpClientFactory.GetClient())
        {
            await ftpClient.AutoConnectAsync();
            await ftpClient.SetWorkingDirectoryAsync(_galleryPath);
            if (await ftpClient.FileExistsAsync(fileName))
            {
                await ftpClient.DeleteFileAsync(fileName);
            }

            return true;
        }
    }

    public async Task<List<string>> SaveFilesAsync(IFormFileCollection files)
    {
        List<string> savedFiles = new List<string>();
        foreach (IFormFile file in files)
        {
            if (file.Length > 0)
            {
                using (var ftpClient = _ftpClientFactory.GetClient())
                {
                    await ftpClient.AutoConnectAsync();
                    await ftpClient.SetWorkingDirectoryAsync(_galleryPath);
                    Guid fileId = Guid.NewGuid();
                    string fileName = fileId.ToString() + Path.GetExtension(file.FileName);
                    try
                    {
                        using (MemoryStream fileStream = new MemoryStream())
                        {
                            await file.CopyToAsync(fileStream);
                            await ftpClient.UploadBytesAsync(fileStream.ToArray(), fileName);
                            savedFiles.Add(fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while saving file to path '{await ftpClient.GetWorkingDirectoryAsync()}'.");
                        continue;
                    }
                }
            }
        }

        return savedFiles;
    }
}