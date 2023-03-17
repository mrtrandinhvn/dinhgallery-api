namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models
{
    public class FileDetailsReadModel
    {
        public Ulid Id { get; set; }
        public Ulid FolderId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public Uri? DownloadUri { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}