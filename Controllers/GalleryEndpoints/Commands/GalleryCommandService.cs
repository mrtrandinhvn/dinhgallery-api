namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;
public class GalleryCommandService : IGalleryCommandService
{
    private readonly ILogger<GalleryCommandService> _logger;
    private readonly IWebHostEnvironment _hostingEnv;

    private string _uploadFolder => Path.Combine(_hostingEnv.WebRootPath, "UserUploadedFiles");

    public GalleryCommandService(
        ILogger<GalleryCommandService> logger,
        IWebHostEnvironment env)
    {
        _logger = logger;
        _hostingEnv = env;
    }

    public bool Delete(string fileName)
    {
        throw new NotImplementedException("Under development");
        string filePath = Path.Combine(_uploadFolder, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return true;
    }

    public async Task<List<string>> SaveFilesAsync(IFormFileCollection files)
    {
        throw new NotImplementedException("Under development");
        List<string> savedFiles = new List<string>();
        foreach (IFormFile file in files)
        {
            if (file.Length > 0)
            {
                Guid fileId = Guid.NewGuid();
                string fileName = fileId.ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(_uploadFolder, fileName);
                try
                {
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        savedFiles.Add(fileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while saving file to path '{filePath}'.");
                    continue;
                }
            }
        }

        return savedFiles;
    }
}