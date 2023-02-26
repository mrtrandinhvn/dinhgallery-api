namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

public interface IGalleryFileWriteRepository
{
    Task<bool> AddAsync(GalleryFileAddInput input);
}