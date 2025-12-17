using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.DbModels;
using Newtonsoft.Json;
using Redis.OM;
using Redis.OM.Searching;

namespace dinhgallery_api.Infrastructures.Repositories;

public class GalleryFileWriteRepository : IGalleryFileWriteRepository
{
    private readonly RedisConnectionProvider _redis;
    private readonly ILogger<GalleryFileWriteRepository> _logger;

    public GalleryFileWriteRepository(
        RedisConnectionProvider redis,
        ILogger<GalleryFileWriteRepository> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<Ulid?> AddAsync(GalleryFileAddInput input)
    {
        ArgumentNullException.ThrowIfNull(input?.DownloadUri);

        var now = DateTime.UtcNow;
        IRedisCollection<FileDbModel> files = _redis.RedisCollection<FileDbModel>();
        FileDbModel entity = new()
        {
            DisplayName = input.DisplayName,
            CreatedAtUtc = now,
            DownloadUri = input.DownloadUri.ToString(),
            FolderId = input.FolderId,
        };
        
        try
        {
            await files.InsertAsync(entity);
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file to db. input: {Input}.", JsonConvert.SerializeObject(input));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Ulid id)
    {
        string key = $"{FileDbModel.TableName}:{id}";
        _logger.LogInformation("Begin deleting key '{Key}'", key);
        var result = await _redis.Connection.UnlinkAsync(key);
        _logger.LogInformation("Finish deleting key '{Key}'. Result is '{Result}'.", key, result);
        return true;
    }
}