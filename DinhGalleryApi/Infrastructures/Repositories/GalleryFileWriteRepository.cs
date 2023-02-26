using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using StackExchange.Redis;

namespace dinhgallery_api.Infrastructures.Repositories;

public class GalleryFileWriteRepository : IGalleryFileWriteRepository
{
    private readonly IConnectionMultiplexer _redis;

    public GalleryFileWriteRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> AddAsync(GalleryFileAddInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(input.DisplayName);
        ArgumentNullException.ThrowIfNull(input.DownloadUri);

        IDatabase db = _redis.GetDatabase();
        var now = DateTime.UtcNow;
        var saveFileTask = db.HashSetAsync($"file:{input.Id}", new HashEntry[]{
            new HashEntry("displayName", input.DisplayName),
            new HashEntry("createdAtUtc", now.ToString("o")),
            new HashEntry("folderId", input.FolderId.ToString()),
            new HashEntry("downloadUri", input.DownloadUri.ToString()),
        });
        var linkWithFolderTask = db.SortedSetAddAsync($"files:{input.FolderId}", input.Id.ToString(), now.Ticks);
        await Task.WhenAll(saveFileTask, linkWithFolderTask);
        return true;
    }
}