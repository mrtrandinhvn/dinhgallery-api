namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories
{
    public class GalleryFileAddInput
    {
        public Guid Id { get; set; }
        public Guid FolderId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public Uri? DownloadUri { get; set; }
    }
}