using dinhgallery_api.BusinessObjects.Commands;
using dinhgallery_api.BusinessObjects.Commands.Decorators;
using dinhgallery_api.BusinessObjects.Options;
using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.BusinessObjects.Queries.Decorators;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.AddFilesToFolder;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.CreateFolderWithFiles;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFile;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.DeleteFolder;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.UpdateFolderDisplayName;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFileDetails;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderDetails;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderList;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
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

        services.AddScoped<IGalleryFolderWriteRepository, GalleryFolderWriteRepository>();
        services.AddScoped<IGalleryFileWriteRepository, GalleryFileWriteRepository>();

        services.AddScoped<IGalleryQueryRepository, GalleryQueryRepository>();
        services.AddScoped<IStorageService, FileSystemStorageService>();
        services.AddScoped<IVideoProcessingService, VideoProcessingService>();

        // Register validators
        services.AddScoped<ICommandValidator<AddFilesToFolderCommand>, AddFilesToFolderCommandValidator>();

        // Register concrete query handlers
        services.AddScoped<GetFolderListQueryHandler>();
        services.AddScoped<GetFolderDetailsQueryHandler>();
        services.AddScoped<GetFileDetailsQueryHandler>();

        // Register concrete command handlers
        services.AddScoped<UpdateFolderDisplayNameCommandHandler>();
        services.AddScoped<AddFilesToFolderCommandHandler>();
        services.AddScoped<CreateFolderWithFilesCommandHandler>();
        services.AddScoped<DeleteFileCommandHandler>();
        services.AddScoped<DeleteFolderCommandHandler>();

        // Register command handlers with decorators
        services.AddScoped<ICommandHandler<UpdateFolderDisplayNameCommand, bool>>(sp =>
        {
            UpdateFolderDisplayNameCommandHandler handler = sp.GetRequiredService<UpdateFolderDisplayNameCommandHandler>();
            ILogger<PerformanceMonitoringCommandHandlerDecorator<UpdateFolderDisplayNameCommand, bool>> performanceLogger = sp.GetRequiredService<ILogger<PerformanceMonitoringCommandHandlerDecorator<UpdateFolderDisplayNameCommand, bool>>>();
            return new PerformanceMonitoringCommandHandlerDecorator<UpdateFolderDisplayNameCommand, bool>(handler, performanceLogger);
        });

        services.AddScoped<ICommandHandler<AddFilesToFolderCommand, Ulid?>>(sp =>
        {
            AddFilesToFolderCommandHandler handler = sp.GetRequiredService<AddFilesToFolderCommandHandler>();
            ValidationCommandHandlerDecorator<AddFilesToFolderCommand, Ulid?> validationDecorator = new (handler, sp.GetRequiredService<ICommandValidator<AddFilesToFolderCommand>>(), sp.GetRequiredService<ILogger<ValidationCommandHandlerDecorator<AddFilesToFolderCommand, Ulid?>>>());
            return new PerformanceMonitoringCommandHandlerDecorator<AddFilesToFolderCommand, Ulid?>(validationDecorator, sp.GetRequiredService<ILogger<PerformanceMonitoringCommandHandlerDecorator<AddFilesToFolderCommand, Ulid?>>>());
        });

        services.AddScoped<ICommandHandler<CreateFolderWithFilesCommand, Ulid?>>(sp =>
        {
            CreateFolderWithFilesCommandHandler handler = sp.GetRequiredService<CreateFolderWithFilesCommandHandler>();
            ILogger<PerformanceMonitoringCommandHandlerDecorator<CreateFolderWithFilesCommand, Ulid?>> performanceLogger = sp.GetRequiredService<ILogger<PerformanceMonitoringCommandHandlerDecorator<CreateFolderWithFilesCommand, Ulid?>>>();
            return new PerformanceMonitoringCommandHandlerDecorator<CreateFolderWithFilesCommand, Ulid?>(handler, performanceLogger);
        });

        services.AddScoped<ICommandHandler<DeleteFileCommand, bool>>(sp =>
        {
            DeleteFileCommandHandler handler = sp.GetRequiredService<DeleteFileCommandHandler>();
            ILogger<PerformanceMonitoringCommandHandlerDecorator<DeleteFileCommand, bool>> performanceLogger = sp.GetRequiredService<ILogger<PerformanceMonitoringCommandHandlerDecorator<DeleteFileCommand, bool>>>();
            return new PerformanceMonitoringCommandHandlerDecorator<DeleteFileCommand, bool>(handler, performanceLogger);
        });

        services.AddScoped<ICommandHandler<DeleteFolderCommand, bool>>(sp =>
        {
            DeleteFolderCommandHandler handler = sp.GetRequiredService<DeleteFolderCommandHandler>();
            ILogger<PerformanceMonitoringCommandHandlerDecorator<DeleteFolderCommand, bool>> performanceLogger = sp.GetRequiredService<ILogger<PerformanceMonitoringCommandHandlerDecorator<DeleteFolderCommand, bool>>>();
            return new PerformanceMonitoringCommandHandlerDecorator<DeleteFolderCommand, bool>(handler, performanceLogger);
        });

        // Register query handlers with performance monitoring decorator
        services.AddScoped<IQueryHandler<GetFolderListQuery, List<FolderDetailsReadModel>>>(sp =>
        {
            GetFolderListQueryHandler handler = sp.GetRequiredService<GetFolderListQueryHandler>();
            ILogger<PerformanceMonitoringQueryHandlerDecorator<GetFolderListQuery, List<FolderDetailsReadModel>>> performanceLogger =
                sp.GetRequiredService<ILogger<PerformanceMonitoringQueryHandlerDecorator<GetFolderListQuery, List<FolderDetailsReadModel>>>>();
            return new PerformanceMonitoringQueryHandlerDecorator<GetFolderListQuery, List<FolderDetailsReadModel>>(handler, performanceLogger);
        });

        services.AddScoped<IQueryHandler<GetFolderDetailsQuery, FolderDetailsReadModel?>>(sp =>
        {
            GetFolderDetailsQueryHandler handler = sp.GetRequiredService<GetFolderDetailsQueryHandler>();
            ILogger<PerformanceMonitoringQueryHandlerDecorator<GetFolderDetailsQuery, FolderDetailsReadModel?>> performanceLogger =
                sp.GetRequiredService<ILogger<PerformanceMonitoringQueryHandlerDecorator<GetFolderDetailsQuery, FolderDetailsReadModel?>>>();
            return new PerformanceMonitoringQueryHandlerDecorator<GetFolderDetailsQuery, FolderDetailsReadModel?>(handler, performanceLogger);
        });

        services.AddScoped<IQueryHandler<GetFileDetailsQuery, FileDetailsResponse?>>(sp =>
        {
            GetFileDetailsQueryHandler handler = sp.GetRequiredService<GetFileDetailsQueryHandler>();
            ILogger<PerformanceMonitoringQueryHandlerDecorator<GetFileDetailsQuery, FileDetailsResponse?>> performanceLogger =
                sp.GetRequiredService<ILogger<PerformanceMonitoringQueryHandlerDecorator<GetFileDetailsQuery, FileDetailsResponse?>>>();
            return new PerformanceMonitoringQueryHandlerDecorator<GetFileDetailsQuery, FileDetailsResponse?>(handler, performanceLogger);
        });
    }

    public static void ConfigureHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<IndexCreationService>();
    }
}