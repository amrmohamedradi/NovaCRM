using Microsoft.Extensions.Caching.Memory;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Infrastructure.Services;

public class InMemoryTokenBlacklistService(IMemoryCache cache) : ITokenBlacklistService
{
    private const string Prefix = "jwt_blacklist:";

    public Task BlacklistAsync(string jti, DateTime expiry, CancellationToken ct = default)
    {
        var ttl = expiry - DateTime.UtcNow;

        if (ttl > TimeSpan.Zero)
        {
            cache.Set(Prefix + jti, true, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(ttl)
            });
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default) =>
        Task.FromResult(cache.TryGetValue(Prefix + jti, out _));
}
