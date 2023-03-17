using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands;

public interface IStorageService
{
    Task<List<GalleryFileAddInput>> SaveAsync(string physicalFolderName, IFormFileCollection files);
    Task<bool> DeleteFolderAsync(string physicalFolderName);
    Task<bool> DeleteFileAsync(Uri absoluteUri);
}