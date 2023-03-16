using dinhgallery_api.DbModels;
using Redis.OM;

namespace dinhgallery_api.HostedServices;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    public IndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(new Task[]{
            _provider.Connection.CreateIndexAsync(typeof(FolderDbModel)),
            _provider.Connection.CreateIndexAsync(typeof(FileDbModel))
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}