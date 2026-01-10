namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

public record FileDetailsResponse(
    Ulid Id,
    string DisplayName,
    Uri? DownloadUri,
    DateTime CreatedAtUtc)
{
    public static FileDetailsResponse FromReadModel(FileDetailsReadModel fileDetails) =>
        new(
            fileDetails.Id,
            fileDetails.DisplayName,
            fileDetails.DownloadUri,
            fileDetails.CreatedAtUtc);
}
