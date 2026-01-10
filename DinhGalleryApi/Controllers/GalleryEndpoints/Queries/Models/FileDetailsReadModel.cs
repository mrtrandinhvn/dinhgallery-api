namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

public record FileDetailsReadModel
{
    public required Ulid Id { get; init; }
    public required Ulid FolderId { get; init; }
    public required string DisplayName { get; init; }
    public Uri? DownloadUri { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}
