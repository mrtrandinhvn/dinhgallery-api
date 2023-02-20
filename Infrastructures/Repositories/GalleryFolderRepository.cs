using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using StackExchange.Redis;

namespace dinhgallery_api.Infrastructures.Repositories;

public class GalleryFolderRepository : IGalleryFolderRepository
{
    private readonly IConnectionMultiplexer _redis;

    public GalleryFolderRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> AddAsync(GalleryFolderAddInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(input.DisplayName);

        IDatabase db = _redis.GetDatabase();
        await db.HashSetAsync($"folder:{input.Id}", new HashEntry[]{
            new HashEntry("displayName", input.DisplayName),
            new HashEntry("createdAtUtc", DateTime.UtcNow.ToString("o")),
        });

        return true;
    }
}