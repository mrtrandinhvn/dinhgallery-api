namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

public interface IGalleryFolderWriteRepository
{
    Task<bool> AddAsync(GalleryFolderAddInput input);
}