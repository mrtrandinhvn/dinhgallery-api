namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories
{
    public class GalleryFileAddInput
    {
        public string? Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public Uri? DownloadUri { get; set; }
        public Ulid FolderId { get; set; }
    }
}