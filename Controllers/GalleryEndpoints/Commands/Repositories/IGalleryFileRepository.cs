namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

public interface IGalleryFileRepository
{
    Task<Guid?> AddAsync(GalleryFileAddInput input);
}