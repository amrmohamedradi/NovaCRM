using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Contacts.Commands;
using NovaCRM.Application.Features.Contacts.Queries;

namespace NovaCRM.API.Controllers;
[ApiController]
[Authorize]
public class ContactsController(IMediator mediator) : ControllerBase
{
    [HttpGet("api/customers/{customerId:guid}/contacts")]
    [ProducesResponseType(typeof(ApiResponse<List<ContactDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<ContactDto>>>> GetByCustomer(Guid customerId)
    {
        var result = await mediator.Send(new GetContactsByCustomerIdQuery(customerId));
        return Ok(ApiResponse<List<ContactDto>>.Ok(result));
    }
    [HttpPost("api/customers/{customerId:guid}/contacts")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<ContactDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<ContactDto>>> Create(Guid customerId, [FromBody] CreateContactRequest request)
    {
        var command = new CreateContactCommand(customerId, request.FullName, request.Email, request.Phone, request.Position);
        var result  = await mediator.Send(command);
        return StatusCode(201, ApiResponse<ContactDto>.Created(result, "Contact created."));
    }
    [HttpPut("api/contacts/{id:guid}")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<ContactDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<ContactDto>>> Update(Guid id, [FromBody] UpdateContactRequest request)
    {
        var command = new UpdateContactCommand(id, request.FullName, request.Email, request.Phone, request.Position);
        var result  = await mediator.Send(command);
        return Ok(ApiResponse<ContactDto>.Ok(result, "Contact updated."));
    }
    [HttpDelete("api/contacts/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await mediator.Send(new DeleteContactCommand(id));
        return Ok(ApiResponse<bool>.Ok(true, "Contact deleted."));
    }
}

public record CreateContactRequest(string FullName, string Email, string? Phone, string? Position);
public record UpdateContactRequest(string FullName, string Email, string? Phone, string? Position);
