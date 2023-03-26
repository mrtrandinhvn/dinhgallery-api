using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using dinhgallery_api.HostedServices;
using dinhgallery_api.Infrastructures;
using dinhgallery_api.Infrastructures.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Redis.OM;
using StackExchange.Redis;

namespace dinhgallery_api.BusinessObjects;

public static class IServiceCollectionExtensions
{
    private const int MAX_REQUEST_BODY_SIZE = 1024 * 1000000;

    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FormOptions>(options =>
        {
            // Set the limit to X MB
            options.MultipartBodyLengthLimit = MAX_REQUEST_BODY_SIZE;
        });
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = MAX_REQUEST_BODY_SIZE; // if don't set default value is ~30 MB
        });
        services.Configure<StorageSettingsOptions>(configuration.GetSection(StorageSettingsOptions.SectionName));
    }

    public static void ConfigureAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<RedisConnectionProvider>(sp =>
        {
            RedisOptions redisOptions = new();
            configuration.GetSection(RedisOptions.SectionName).Bind(redisOptions);
            ArgumentNullException.ThrowIfNull(redisOptions.Host);
            ArgumentNullException.ThrowIfNull(redisOptions.Port);
            ArgumentNullException.ThrowIfNull(redisOptions.Password);
            return new RedisConnectionProvider(new ConfigurationOptions
            {
                EndPoints = { redisOptions.Host + ":" + redisOptions.Port },
                Password = redisOptions.Password,
            });
        });

        services.AddScoped<FtpClientFactory>();

        services.AddScoped<IGalleryCommandService, GalleryCommandService>();
        services.AddScoped<IGalleryQueryService, GalleryQueryService>();
        services.AddScoped<IGalleryFolderWriteRepository, GalleryFolderWriteRepository>();
        services.AddScoped<IGalleryFileWriteRepository, GalleryFileWriteRepository>();

        services.AddScoped<IGalleryQueryRepository, GalleryQueryRepository>();
        services.AddScoped<IGalleryQueryService, GalleryQueryService>();
        services.AddScoped<IStorageService, StorageService>();
    }

    public static void ConfigureHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<IndexCreationService>();
    }
}