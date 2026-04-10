using MediatR;
using Microsoft.AspNetCore.Identity;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Auth.Commands;
using NovaCRM.Application.Interfaces;
using NovaCRM.Infrastructure.Data;

namespace NovaCRM.Infrastructure.Handlers;
public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtService jwtService)
    : IRequestHandler<LoginCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? "Viewer";
        var token = jwtService.GenerateToken(user.Id, user.Email!, user.FullName, role);

        var refreshToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        await userManager.SetAuthenticationTokenAsync(user, "NovaCRMProvider", "RefreshToken", refreshToken);

        return new AuthResultDto 
        { 
            Token = token, 
            RefreshToken = refreshToken,
            Email = user.Email!, 
            FullName = user.FullName, 
            Role = role 
        };
    }
}
