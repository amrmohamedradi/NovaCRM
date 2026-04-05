using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Dashboard.Queries;

namespace NovaCRM.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), 200)]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
    {
        var result = await mediator.Send(new GetDashboardStatsQuery());
        return Ok(ApiResponse<DashboardStatsDto>.Ok(result));
    }
}



