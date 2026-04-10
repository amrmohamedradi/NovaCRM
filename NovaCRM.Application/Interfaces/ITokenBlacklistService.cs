namespace NovaCRM.Application.Interfaces;

public interface ITokenBlacklistService
{

    Task BlacklistAsync(string jti, DateTime expiry, CancellationToken ct = default);

    Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default);
}
