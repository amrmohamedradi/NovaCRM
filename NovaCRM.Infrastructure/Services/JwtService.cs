using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Infrastructure.Services;
public class JwtService(IConfiguration config) : IJwtService
{
    public string GenerateToken(string userId, string email, string fullName, string role)
    {
        var secret   = config["JwtSettings:Secret"]!;
        var issuer   = config["JwtSettings:Issuer"]!;
        var audience = config["JwtSettings:Audience"]!;
        var expiryDays = int.Parse(config["JwtSettings:ExpiryDays"] ?? "7");

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("fullName",                    fullName),
            new Claim(ClaimTypes.Role,               role),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddDays(expiryDays),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
