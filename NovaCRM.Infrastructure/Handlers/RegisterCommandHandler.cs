using MediatR;
using Microsoft.AspNetCore.Identity;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Auth.Commands;
using NovaCRM.Application.Interfaces;
using NovaCRM.Infrastructure.Data;

namespace NovaCRM.Infrastructure.Handlers;
public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService)
    : IRequestHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken ct)
    {

        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email    = request.Email,
            FullName = request.FullName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        await userManager.AddToRoleAsync(user, "Sales");

        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? "Sales";
        var token = jwtService.GenerateToken(user.Id, user.Email!, user.FullName, role);

        return new AuthResultDto { Token = token, Email = user.Email!, FullName = user.FullName, Role = role };
    }
}
