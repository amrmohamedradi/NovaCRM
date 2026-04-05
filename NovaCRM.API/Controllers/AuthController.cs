using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Auth.Commands;

namespace NovaCRM.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Register([FromBody] RegisterCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(201, ApiResponse<AuthResultDto>.Created(result, "Registration successful."));
    }
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(ApiResponse<AuthResultDto>.Ok(result, "Login successful."));
    }
}



