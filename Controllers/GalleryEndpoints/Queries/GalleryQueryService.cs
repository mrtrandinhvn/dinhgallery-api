using dinhgallery_api.BusinessObjects;
using dinhgallery_api.BusinessObjects.Options;
using FluentFTP;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

public class GalleryQueryService : IGalleryQueryService
{
    private const string _galleryPath = "/home/www/gallery/";
    private readonly PublicAppSettingsOptions _appSettingsOptions;
    private readonly FtpClientFactory _ftpClientFactory;

    public GalleryQueryService(
        IOptions<PublicAppSettingsOptions> appSettingsOptions,
        FtpClientFactory ftpClientFactory)
    {
        _appSettingsOptions = appSettingsOptions.Value;
        this._ftpClientFactory = ftpClientFactory;
    }

    public async Task<List<Uri>> GetAllUrisAsync()
    {
        using (FtpClient ftpClient = _ftpClientFactory.GetClient())
        {
            await ftpClient.AutoConnectAsync();
            List<Uri> uris = (await ftpClient.GetNameListingAsync(_galleryPath))
                .Select(fileName => new Uri($"{_appSettingsOptions.StorageServiceBaseUrl}/{fileName.Replace("/home/www/", string.Empty)}", UriKind.Absolute))
                .ToList();

            return uris;
        }
    }

    public Uri GetUriByName(string fileName)
    {
        return new Uri($"{_appSettingsOptions.StorageServiceBaseUrl}/gallery/{fileName}");
    }
}