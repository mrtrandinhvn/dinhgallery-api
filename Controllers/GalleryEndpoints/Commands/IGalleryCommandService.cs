namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public interface IGalleryCommandService
{
    Task<List<string>> SaveFilesAsync(IFormFileCollection files);
    Task<bool> DeleteAsync(string fileName);
}