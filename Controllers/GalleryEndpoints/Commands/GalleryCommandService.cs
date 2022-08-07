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

    public Task<List<string>> SaveFilesAsync(IFormFileCollection files)
    {
        throw new NotImplementedException("Under development");
        List<string> savedFiles = new List<string>();
        // foreach (IFormFile file in files)
        // {
        //     if (file.Length > 0)
        //     {
        //         Guid fileId = Guid.NewGuid();
        //         string fileName = fileId.ToString() + Path.GetExtension(file.FileName);
        //         string filePath = Path.Combine(_uploadFolder, fileName);
        //         try
        //         {
        //             using (Stream fileStream = new FileStream(filePath, FileMode.Create))
        //             {
        //                 await file.CopyToAsync(fileStream);
        //                 savedFiles.Add(fileName);
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             _logger.LogError(ex, $"Error while saving file to path '{filePath}'.");
        //             continue;
        //         }
        //     }
        // }

        return Task.FromResult(savedFiles);
    }
}