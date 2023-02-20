namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

public interface IGalleryFileRepository
{
    Task<bool> AddAsync(GalleryFileAddInput input);
}