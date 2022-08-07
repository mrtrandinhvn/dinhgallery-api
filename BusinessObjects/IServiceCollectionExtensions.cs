using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace dinhgallery_api.BusinessObjects;

public static class IServiceCollectionExtensions
{

    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FormOptions>(options =>
        {
            // Set the limit to 500 MB
            options.MultipartBodyLengthLimit = 524_288_000;
        });
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 524_288_000; // if don't set default value is ~30 MB
        });
        services.Configure<PublicAppSettingsOptions>(configuration.GetSection(PublicAppSettingsOptions.SectionName));
    }

    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IGalleryCommandService, GalleryCommandService>();
        services.AddScoped<IGalleryQueryService, GalleryQueryService>();
        services.AddScoped<FtpClientFactory>();
    }
}