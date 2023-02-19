using dinhgallery_api.BusinessObjects;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public class GalleryCommandService : IGalleryCommandService
{
    private const string _galleryPath = "/home/www/gallery/";
    private readonly FtpClientFactory _ftpClientFactory;
    private readonly IGalleryFolderRepository _folderRepository;
    private readonly IGalleryFileRepository _fileRepository;
    private readonly ILogger<GalleryCommandService> _logger;

    public GalleryCommandService(
        ILogger<GalleryCommandService> logger,
        FtpClientFactory ftpClientFactory,
        IGalleryFolderRepository folderRepository,
        IGalleryFileRepository fileRepository)
    {
        _logger = logger;
        _ftpClientFactory = ftpClientFactory;
        _folderRepository = folderRepository;
        _fileRepository = fileRepository;
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

    public async Task<List<string>> SaveFilesAsync(string folderDisplayName, IFormFileCollection files)
    {
        List<string> savedFiles = new List<string>();
        Guid folderId = Guid.NewGuid();
        await _folderRepository.AddAsync(new GalleryFolderAddInput
        {
            Id = folderId,
            DisplayName = folderDisplayName ?? folderId.ToString(),
        });

        using (var ftpClient = _ftpClientFactory.GetClient())
        {
            await ftpClient.AutoConnectAsync();
            await ftpClient.CreateDirectoryAsync(folderId.ToString());
            await ftpClient.SetWorkingDirectoryAsync(_galleryPath + folderId + "/");
            foreach (IFormFile file in files)
            {
                if (file.Length > 0)
                {
                    Guid fileId = Guid.NewGuid();
                    string fileName = fileId.ToString() + Path.GetExtension(file.FileName);
                    try
                    {
                        using (MemoryStream fileStream = new())
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

                    await _fileRepository.AddAsync(new GalleryFileAddInput
                    {
                        Id = fileId,
                        FolderId = folderId,
                        DisplayName = file.FileName,
                    });
                }
            }
        }

        return savedFiles;
    }
}