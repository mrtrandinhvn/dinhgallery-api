namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public interface IGalleryCommandService
{
    Task<Guid> SaveFilesAsync(SaveFilesInput input);
    Task<bool> DeleteAsync(string fileId);
}