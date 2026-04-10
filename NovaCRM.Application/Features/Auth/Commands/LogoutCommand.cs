using MediatR;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Auth.Commands;

public record LogoutCommand : IRequest<bool>;

public class LogoutCommandHandler(
    ICurrentUserService currentUser,
    ITokenBlacklistService blacklist)
    : IRequestHandler<LogoutCommand, bool>
{
    public async Task<bool> Handle(LogoutCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.Jti is null)
            return false;

        var expiry = currentUser.TokenExpiry
                     ?? DateTime.UtcNow.AddDays(7);

        await blacklist.BlacklistAsync(currentUser.Jti, expiry, ct);
        return true;
    }
}
