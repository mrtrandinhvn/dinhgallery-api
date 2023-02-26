using dinhgallery_api.BusinessObjects;
using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using FluentFTP;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

public class GalleryQueryService : IGalleryQueryService
{
    private const string _galleryPath = "/home/www/gallery/";
    private readonly StorageSettingsOptions _appSettingsOptions;
    private readonly FtpClientFactory _ftpClientFactory;
    private readonly IGalleryQueryRepository _queryRepository;

    public GalleryQueryService(
        IOptions<StorageSettingsOptions> appSettingsOptions,
        FtpClientFactory ftpClientFactory,
        IGalleryQueryRepository queryRepository)
    {
        _appSettingsOptions = appSettingsOptions.Value;
        this._ftpClientFactory = ftpClientFactory;
        this._queryRepository = queryRepository;
    }

    public Task<FileDetailsReadModel> GetFileDetailsAsync(Guid fileId)
    {
        return _queryRepository.GetFileDetailsAsync(fileId);
    }

    public Task<FolderDetailsReadModel> GetFolderDetailsAsync(Guid folderId)
    {
        return _queryRepository.GetFolderDetailsAsync(folderId);
    }

    public Task<List<Guid>> GetFolderListAsync()
    {
        return _queryRepository.GetFolderListAsync();
    }
}