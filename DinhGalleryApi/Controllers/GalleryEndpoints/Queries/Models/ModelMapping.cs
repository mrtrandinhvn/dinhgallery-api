using dinhgallery_api.DbModels;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;

public static class ModelMapping
{
    public static FileDetailsReadModel ToReadModel(this FileDbModel model)
    {
        return new FileDetailsReadModel
        {
            CreatedAtUtc = model.CreatedAtUtc,
            DisplayName = model.DisplayName,
            DownloadUri = string.IsNullOrEmpty(model.DownloadUri) ? null : new Uri(model.DownloadUri, UriKind.Absolute),
            Id = model.Id,
            FolderId = model.FolderId,
        };
    }

    public static FolderDetailsReadModel ToReadModel(this FolderDbModel model)
    {
        return new FolderDetailsReadModel
        {
            CreatedAtUtc = model.CreatedAtUtc,
            DisplayName = model.DisplayName,
            Id = model.Id,
            PhysicalName = model.PhysicalFolderName,
        };
    }
}