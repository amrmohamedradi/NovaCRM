namespace NovaCRM.Application.Interfaces;

public interface ICurrentUserService
{

    string? UserId { get; }

    string? Email { get; }

    string? FullName { get; }

    string? Role { get; }

    string? Jti { get; }

    DateTime? TokenExpiry { get; }

    bool IsAuthenticated { get; }
}
