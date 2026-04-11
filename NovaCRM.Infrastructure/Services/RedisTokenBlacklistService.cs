using Microsoft.Extensions.Caching.Distributed;
using NovaCRM.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NovaCRM.Infrastructure.Services;

public class RedisTokenBlacklistService(IDistributedCache cache) : ITokenBlacklistService
{
    private const string Prefix = "jwt_blacklist:";

    public async Task BlacklistAsync(string jti, DateTime expiry, CancellationToken ct = default)
    {
        var ttl = expiry - DateTime.UtcNow;
        if (ttl > TimeSpan.Zero)
        {
            await cache.SetStringAsync(Prefix + jti, "revoked", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            }, ct);
        }
    }

    public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default)
    {
        var value = await cache.GetStringAsync(Prefix + jti, ct);
        return value is not null;
    }
}
