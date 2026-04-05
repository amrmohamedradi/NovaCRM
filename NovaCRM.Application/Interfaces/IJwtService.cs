namespace NovaCRM.Application.Interfaces;
public interface IJwtService
{
    string GenerateToken(string userId, string email, string fullName, string role);
}



