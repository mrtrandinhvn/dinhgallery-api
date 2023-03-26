namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands
{
    public class SaveFilesInput
    {
        public List<IFormFile>? FormFiles { get; set; }
        public string? FolderDisplayName { get; set; }
    }
}