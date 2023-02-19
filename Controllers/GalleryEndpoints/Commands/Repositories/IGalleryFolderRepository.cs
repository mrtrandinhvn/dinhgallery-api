namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

public interface IGalleryFolderRepository
{
    Task<bool> AddAsync(GalleryFolderAddInput input);
}