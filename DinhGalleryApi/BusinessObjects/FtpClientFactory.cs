using dinhgallery_api.BusinessObjects.Options;
using FluentFTP;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.BusinessObjects;

public class FtpClientFactory
{
    private readonly StorageSettingsOptions _publicAppSettingsOptions;

    public FtpClientFactory(IOptions<StorageSettingsOptions> publicAppSettingsOptions)
    {
        this._publicAppSettingsOptions = publicAppSettingsOptions.Value;
    }

    public FtpClient GetClient()
    {
        FtpClient ftpClient = new(_publicAppSettingsOptions.FtpHost, _publicAppSettingsOptions.FtpUsername, _publicAppSettingsOptions.FtpPassword);
        return ftpClient;
    }
}