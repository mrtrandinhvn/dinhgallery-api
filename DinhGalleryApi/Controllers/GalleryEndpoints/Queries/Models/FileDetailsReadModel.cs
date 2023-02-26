namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models
{
    public class FileDetailsReadModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public Uri? DownloadUri { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}