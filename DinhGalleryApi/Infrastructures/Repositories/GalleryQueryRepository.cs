using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.DbModels;
using Redis.OM;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

public class GalleryQueryRepository : IGalleryQueryRepository
{
    private readonly RedisConnectionProvider _redis;

    public GalleryQueryRepository(RedisConnectionProvider redis)
    {
        _redis = redis;
    }

    public async Task<FileDetailsReadModel?> GetFileDetailsAsync(Ulid fileId)
    {
        return (await _redis.RedisCollection<FileDbModel>()
            .Where(x => x.Id == fileId)
            .FirstOrDefaultAsync())
            ?.ToReadModel();
    }

    public async Task<FolderDetailsReadModel?> GetFolderDetailsAsync(Ulid folderId)
    {
        var searchFolderTask = (await _redis.RedisCollection<FolderDbModel>()
            .Where(x => x.Id == folderId)
            .FirstOrDefaultAsync()
            )?.ToReadModel();
        var searchFilesTask = (await _redis.RedisCollection<FileDbModel>()
            .Where(x => x.FolderId == folderId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync()
            ).Select(x => x.ToReadModel());

        FolderDetailsReadModel? folder = searchFolderTask;
        if (folder is null)
        {
            return null;
        }

        return folder with { Files = searchFilesTask };
    }

    public async Task<(List<FolderDetailsReadModel> Items, int TotalCount)> GetFolderListAsync(int skip, int take)
    {
        var collection = _redis.RedisCollection<FolderDbModel>();

        var totalCount = await collection.CountAsync();

        var items = (await collection
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync())
            .Select(x => x.ToReadModel())
            .ToList();

        return (items, (int)totalCount);
    }
}