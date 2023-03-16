namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models
{
    public class FolderDetailsReadModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<FileDetailsReadModel> Files { get; set; } = Enumerable.Empty<FileDetailsReadModel>();
        public DateTime CreatedAtUtc { get; set; }
        public Ulid Id { get; set; }
        public string PhysicalName { get; set; } = string.Empty;
    }
}