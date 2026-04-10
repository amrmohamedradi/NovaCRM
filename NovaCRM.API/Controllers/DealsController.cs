using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Deals.Commands;
using NovaCRM.Application.Features.Deals.Queries;
using NovaCRM.Domain.Enums;

namespace NovaCRM.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DealsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DealDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<DealDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DealStage? stage = null)
    {
        var result = await mediator.Send(new GetAllDealsQuery(page, pageSize, stage));
        return Ok(ApiResponse<PagedResult<DealDto>>.Ok(result));
    }
    [HttpGet("pipeline")]
    [ProducesResponseType(typeof(ApiResponse<List<DealPipelineDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<DealPipelineDto>>>> GetPipeline()
    {
        var result = await mediator.Send(new GetDealsPipelineQuery());
        return Ok(ApiResponse<List<DealPipelineDto>>.Ok(result));
    }
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DealDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<DealDto>>> GetById(Guid id)
    {
        var result = await mediator.Send(new GetDealByIdQuery(id));
        return Ok(ApiResponse<DealDto>.Ok(result));
    }
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<DealDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<DealDto>>> Create([FromBody] CreateDealCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(201, ApiResponse<DealDto>.Created(result, "Deal created."));
    }
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<DealDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<DealDto>>> Update(Guid id, [FromBody] UpdateDealRequest request)
    {
        var command = new UpdateDealCommand(id, request.Title, request.Value, request.Stage, request.ExpectedCloseDate);
        var result  = await mediator.Send(command);
        return Ok(ApiResponse<DealDto>.Ok(result, "Deal updated."));
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await mediator.Send(new DeleteDealCommand(id));
        return Ok(ApiResponse<bool>.Ok(true, "Deal deleted."));
    }
}

public record UpdateDealRequest(string Title, decimal Value, DealStage Stage, DateTime? ExpectedCloseDate);
