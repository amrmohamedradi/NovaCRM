using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Auth.Commands;

namespace NovaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Register(
        [FromBody] RegisterCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(201, ApiResponse<AuthResultDto>.Created(result, "Registration successful."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Login(
        [FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(ApiResponse<AuthResultDto>.Ok(result, "Login successful."));
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        await mediator.Send(new LogoutCommand());
        return Ok(ApiResponse<object>.Ok(null!, "Logged out successfully. Token has been revoked."));
    }
}
