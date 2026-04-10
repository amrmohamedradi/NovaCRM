using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Activities.Commands;
using NovaCRM.Application.Features.Activities.Queries;

namespace NovaCRM.API.Controllers;
[ApiController]
[Authorize]
public class ActivitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet("api/customers/{customerId:guid}/activities")]
    [ProducesResponseType(typeof(ApiResponse<List<ActivityDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<ActivityDto>>>> GetByCustomer(Guid customerId)
    {
        var result = await mediator.Send(new GetActivitiesByCustomerIdQuery(customerId));
        return Ok(ApiResponse<List<ActivityDto>>.Ok(result));
    }
    [HttpGet("api/deals/{dealId:guid}/activities")]
    [ProducesResponseType(typeof(ApiResponse<List<ActivityDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<ActivityDto>>>> GetByDeal(Guid dealId)
    {
        var result = await mediator.Send(new GetActivitiesByDealIdQuery(dealId));
        return Ok(ApiResponse<List<ActivityDto>>.Ok(result));
    }
    [HttpPost("api/activities")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<ActivityDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<ActivityDto>>> Create([FromBody] CreateActivityCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(201, ApiResponse<ActivityDto>.Created(result, "Activity created."));
    }
    [HttpPut("api/activities/{id:guid}/done")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> MarkDone(Guid id)
    {
        await mediator.Send(new MarkActivityAsDoneCommand(id));
        return Ok(ApiResponse<bool>.Ok(true, "Activity marked as done."));
    }
    [HttpDelete("api/activities/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await mediator.Send(new DeleteActivityCommand(id));
        return Ok(ApiResponse<bool>.Ok(true, "Activity deleted."));
    }
}
