using dinhgallery_api.DbModels;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace dinhgallery_api.HostedServices;

public class IndexCreationService : IHostedService
{
    private readonly IRedisConnectionProvider _provider;

    public IndexCreationService(IRedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _provider.Connection.DropIndexAsync(typeof(FolderDbModel));
        }
        catch (RedisServerException ex) when (ex.Message.Contains("Unknown Index name", StringComparison.OrdinalIgnoreCase))
        {
            // The folder index does not exist on a fresh installation.
        }

        await Task.WhenAll(
            _provider.Connection.CreateIndexAsync(typeof(FolderDbModel)),
            _provider.Connection.CreateIndexAsync(typeof(FileDbModel)));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
