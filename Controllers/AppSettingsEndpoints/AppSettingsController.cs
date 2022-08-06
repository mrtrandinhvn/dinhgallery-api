using dinhgallery_api.BusinessObjects.Constants;
using dinhgallery_api.BusinessObjects.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Controllers.AppSettingsEndpoints;

[Authorize(Roles = AppRole.Admin)]
[ApiController]
[Route("[controller]")]
public class AppSettingsController : ControllerBase
{
    private readonly IOptions<PublicAppSettingsOptions> _options;

    public AppSettingsController(IOptions<PublicAppSettingsOptions> publicAppSettingsOptions)
    {
        this._options = publicAppSettingsOptions;
    }

    [HttpGet]
    public MyAppSettingsResponseModel Get()
    {
        return new MyAppSettingsResponseModel
        {
            FtpHost = _options.Value.FtpHost,
            FtpPassword = _options.Value.FtpPassword,
            FtpUsername = _options.Value.FtpUsername,
            StorageServiceBaseUrl = _options.Value.StorageServiceBaseUrl,
        };
    }
}