namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

public interface IGalleryFolderWriteRepository
{
    Task<Ulid?> AddAsync(GalleryFolderAddInput input);
    Task<bool> UpdateAsync(UpdateFolderDisplayNameInput input);
    Task<bool> DeleteAsync(Ulid folderId);
}