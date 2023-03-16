namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

public interface IGalleryFileWriteRepository
{
    Task<Ulid?> AddAsync(GalleryFileAddInput input);
    Task<bool> DeleteAsync(Ulid fileId);
}