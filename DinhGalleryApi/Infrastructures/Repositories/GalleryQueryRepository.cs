using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using StackExchange.Redis;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

public class GalleryQueryRepository : IGalleryQueryRepository
{
    private readonly IConnectionMultiplexer _redis;

    public GalleryQueryRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public Task<FileDetailsReadModel> GetFileDetailsAsync(Guid fileId)
    {
        throw new NotImplementedException();
    }

    public async Task<FolderDetailsReadModel> GetFolderDetailsAsync(Guid folderId)
    {
        IDatabase db = _redis.GetDatabase();
        FolderDetailsReadModel result = MapHashEntryToFolderDetails(new FolderDetailsReadModel(), await db.HashGetAllAsync($"folder:{folderId}"));
        var folderFileIds = await db.SortedSetRangeByScoreAsync($"files:{folderId}");
        List<Task<HashEntry[]>> getFileDetailsTasks = new();
        foreach (var fileId in folderFileIds)
        {
            getFileDetailsTasks.Add(db.HashGetAllAsync($"file:{fileId}"));
        }
        HashEntry[][] fileDetails = await Task.WhenAll(getFileDetailsTasks);
        result.Files = fileDetails.Select((entries, i) =>
        {
            FileDetailsReadModel fileReadModel = new()
            {
                Id = Guid.Parse(folderFileIds[i].ToString()),
            };
            MapHashEntryToFileDetails(fileReadModel, entries);
            return fileReadModel;
        })
        .ToList();

        return result;
    }

    public Task<List<Guid>> GetFolderListAsync()
    {
        throw new NotImplementedException();
    }

    private static FolderDetailsReadModel MapHashEntryToFolderDetails(FolderDetailsReadModel seed, IEnumerable<HashEntry> entries)
    {
        foreach (HashEntry entry in entries)
        {
            switch (entry.Name)
            {
                case "displayName":
                    seed.DisplayName = entry.Value.ToString();
                    break;

                case "createdAtUtc":
                    seed.CreatedAtUtc = DateTime.Parse(entry.Value.ToString()).ToUniversalTime();
                    break;
            }
        }

        return seed;
    }

    private static FileDetailsReadModel MapHashEntryToFileDetails(FileDetailsReadModel seed, IEnumerable<HashEntry> entries)
    {
        foreach (HashEntry entry in entries)
        {
            switch (entry.Name)
            {
                case "displayName":
                    seed.DisplayName = entry.Value.ToString();
                    break;

                case "createdAtUtc":
                    seed.CreatedAtUtc = DateTime.Parse(entry.Value.ToString()).ToUniversalTime();
                    break;

                case "downloadUri":
                    seed.DownloadUri = new Uri(entry.Value.ToString());
                    break;
            }
        }

        return seed;
    }
}