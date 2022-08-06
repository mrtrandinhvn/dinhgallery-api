using dinhgallery_api.BusinessObjects.Options;
using FluentFTP;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

public class GalleryQueryService : IGalleryQueryService
{
    private const string _galleryPath = "/home/www/gallery/";
    private readonly PublicAppSettingsOptions _appSettingsOptions;
    private readonly FtpClient _ftpClient;

    public GalleryQueryService(
        IOptions<PublicAppSettingsOptions> appSettingsOptions,
        FtpClient ftpClient)
    {
        _appSettingsOptions = appSettingsOptions.Value;
        this._ftpClient = ftpClient;
    }

    public async Task<List<Uri>> GetAllUrisAsync()
    {
        await _ftpClient.AutoConnectAsync();
        List<Uri> uris = (await _ftpClient.GetNameListingAsync(_galleryPath))
            .Select(fileName => new Uri($"{_appSettingsOptions.StorageServiceBaseUrl}/{fileName.Replace("/home/www/", string.Empty)}", UriKind.Absolute))
            .ToList();

        _ftpClient.Dispose();
        return uris;
    }

    public Uri GetUriByName(string fileName)
    {
        return new Uri($"{_appSettingsOptions.StorageServiceBaseUrl}/gallery/{fileName}");
    }
}