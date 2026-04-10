using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Notes.Commands;
using NovaCRM.Application.Features.Notes.Queries;

namespace NovaCRM.API.Controllers;
[ApiController]
[Authorize]
public class NotesController(IMediator mediator) : ControllerBase
{
    [HttpGet("api/customers/{customerId:guid}/notes")]
    [ProducesResponseType(typeof(ApiResponse<List<NoteDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<NoteDto>>>> GetByCustomer(Guid customerId)
    {
        var result = await mediator.Send(new GetNotesByCustomerIdQuery(customerId));
        return Ok(ApiResponse<List<NoteDto>>.Ok(result));
    }
    [HttpGet("api/notes/follow-ups")]
    [ProducesResponseType(typeof(ApiResponse<List<NoteDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<NoteDto>>>> GetFollowUps()
    {
        var result = await mediator.Send(new GetUpcomingFollowUpsQuery());
        return Ok(ApiResponse<List<NoteDto>>.Ok(result));
    }
    [HttpPost("api/customers/{customerId:guid}/notes")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<NoteDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<NoteDto>>> Create(Guid customerId, [FromBody] CreateNoteRequest request)
    {
        var command = new CreateNoteCommand(customerId, request.Content, request.FollowUpDate);
        var result  = await mediator.Send(command);
        return StatusCode(201, ApiResponse<NoteDto>.Created(result, "Note created."));
    }
    [HttpDelete("api/notes/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await mediator.Send(new DeleteNoteCommand(id));
        return Ok(ApiResponse<bool>.Ok(true, "Note deleted."));
    }
}

public record CreateNoteRequest(string Content, DateTime? FollowUpDate);
