namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;
public interface IGalleryQueryService
{
    Task<List<Uri>> GetAllUrisAsync();
    Uri GetUriByName(string fileName);
}