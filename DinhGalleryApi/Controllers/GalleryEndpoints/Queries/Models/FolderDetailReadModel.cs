namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

public record FolderDetailsReadModel
{
    public required Ulid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string PhysicalName { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public IEnumerable<FileDetailsReadModel> Files { get; init; } = [];
}
