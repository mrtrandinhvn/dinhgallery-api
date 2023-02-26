using StackExchange.Redis;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models
{
    public class FolderDetailsReadModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public List<FileDetailsReadModel> Files { get; set; } = new();
        public DateTime CreatedAtUtc { get; set; }
    }
}