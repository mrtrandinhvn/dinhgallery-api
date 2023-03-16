namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models
{
    public sealed class FileDetailsResponse
    {
        public FileDetailsResponse(FileDetailsReadModel fileDetails)
        {
            this.CreatedAtUtc = fileDetails.CreatedAtUtc;
            this.DisplayName = fileDetails.DisplayName;
            this.DownloadUri = fileDetails.DownloadUri;
            this.Id = fileDetails.Id;
        }

        public DateTime CreatedAtUtc { get; private set; }
        public string DisplayName { get; private set; }
        public Uri? DownloadUri { get; private set; }
        public Ulid Id { get; private set; }
    }
}