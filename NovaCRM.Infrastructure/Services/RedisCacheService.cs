using Microsoft.Extensions.Caching.Distributed;
using NovaCRM.Application.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NovaCRM.Infrastructure.Services;

public class RedisCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var cachedData = await cache.GetStringAsync(key, ct);
        if (cachedData is null)
            return default;

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }

        var serializedData = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, serializedData, options, ct);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        return cache.RemoveAsync(key, ct);
    }
}
