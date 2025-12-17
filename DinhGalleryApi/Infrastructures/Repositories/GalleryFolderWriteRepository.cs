using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.DbModels;
using Newtonsoft.Json;
using Redis.OM;
using Redis.OM.Searching;

namespace dinhgallery_api.Infrastructures.Repositories;

public class GalleryFolderWriteRepository : IGalleryFolderWriteRepository
{
    private readonly RedisConnectionProvider _redis;
    private readonly ILogger<GalleryFolderWriteRepository> _logger;

    public GalleryFolderWriteRepository(
        RedisConnectionProvider redis,
        ILogger<GalleryFolderWriteRepository> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<Ulid?> AddAsync(GalleryFolderAddInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(input.DisplayName);

        var now = DateTime.UtcNow;
        IRedisCollection<FolderDbModel> folders = _redis.RedisCollection<FolderDbModel>();
        FolderDbModel entity = new()
        {
            DisplayName = input.DisplayName,
            CreatedAtUtc = DateTime.UtcNow,
            PhysicalFolderName = input.PhysicalName,
        };
        try
        {
            await folders.InsertAsync(entity);
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save folder to db. input: {Input}.", JsonConvert.SerializeObject(input));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Ulid folderId)
    {
        string key = $"{FolderDbModel.TableName}:{folderId}";
        _logger.LogInformation("Begin deleting key '{Key}'", key);
        var result = await _redis.Connection.UnlinkAsync(key);
        _logger.LogInformation("Finish deleting key '{Key}'. Result is '{Result}'.", key, result);
        return true;
    }
}