namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public interface IGalleryCommandService
{
    Task<List<string>> SaveFilesAsync(string folderDisplayName, IFormFileCollection files);
    Task<bool> DeleteAsync(string fileId);
}