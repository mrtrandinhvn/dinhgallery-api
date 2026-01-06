namespace dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories
{
    public class UpdateFolderDisplayNameInput
    {
        public Ulid FolderId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
