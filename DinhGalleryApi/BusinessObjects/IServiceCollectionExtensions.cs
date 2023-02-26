using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using dinhgallery_api.Infrastructures.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using StackExchange.Redis;

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
        services.Configure<StorageSettingsOptions>(configuration.GetSection(StorageSettingsOptions.SectionName));
        services.Configure<HashSettingsOptions>(configuration.GetSection(HashSettingsOptions.SectionName));
    }

    public static void ConfigureAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
        });

        services.AddScoped<FtpClientFactory>();

        services.AddScoped<IGalleryCommandService, GalleryCommandService>();
        services.AddScoped<IGalleryQueryService, GalleryQueryService>();
        services.AddScoped<IGalleryFolderWriteRepository, GalleryFolderWriteRepository>();
        services.AddScoped<IGalleryFileWriteRepository, GalleryFileWriteRepository>();

        services.AddScoped<IGalleryQueryRepository, GalleryQueryRepository>();
        services.AddScoped<IGalleryQueryService, GalleryQueryService>();
    }
}