using Microsoft.Extensions.Diagnostics.HealthChecks;
using Redis.OM;

namespace dinhgallery_api.Infrastructures.HealthChecks;

public sealed class RedisHealthCheck : IHealthCheck
{
    private readonly RedisConnectionProvider _provider;

    public RedisHealthCheck(RedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _provider.Connection.ExecuteAsync("PING");
            return HealthCheckResult.Healthy("Redis is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unavailable.", ex);
        }
    }
}
