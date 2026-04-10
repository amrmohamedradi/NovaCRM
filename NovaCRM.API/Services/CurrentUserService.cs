using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    private ClaimsPrincipal? User =>
        httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

    public string? Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue(JwtRegisteredClaimNames.Email);

    public string? FullName =>
        User?.FindFirstValue("fullName");

    public string? Role =>
        User?.FindFirstValue(ClaimTypes.Role);

    public string? Jti =>
        User?.FindFirstValue(JwtRegisteredClaimNames.Jti);

    public DateTime? TokenExpiry
    {
        get
        {
            var exp = User?.FindFirstValue(JwtRegisteredClaimNames.Exp)
                      ?? User?.FindFirstValue("exp");

            return long.TryParse(exp, out var unix)
                ? DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime
                : null;
        }
    }

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;
}
