namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public interface IGalleryCommandService
{
    Task<Ulid?> SaveFilesAsync(SaveFilesInput input);
    Task<bool> UpdateFolderDisplayNameAsync(Ulid folderId, string displayName);
    Task<bool> DeleteFolderAsync(Ulid folderId);
    Task<bool> DeleteFileAsync(Ulid fileId);
}