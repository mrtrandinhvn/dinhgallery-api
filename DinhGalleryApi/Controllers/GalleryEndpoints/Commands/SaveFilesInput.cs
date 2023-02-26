namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands
{
    public class SaveFilesInput
    {
        public IFormFileCollection? FormFiles { get; set; }
        public string? FolderDisplayName { get; set; }
    }
}