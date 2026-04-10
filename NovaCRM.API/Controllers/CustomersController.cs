using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Customers.Commands;
using NovaCRM.Application.Features.Customers.Queries;
using NovaCRM.Domain.Enums;

namespace NovaCRM.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CustomerDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await mediator.Send(new GetAllCustomersQuery(page, pageSize, search));
        return Ok(ApiResponse<PagedResult<CustomerDto>>.Ok(result));
    }
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<CustomerDetailDto>>> GetById(Guid id)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id));
        return Ok(ApiResponse<CustomerDetailDto>.Ok(result));
    }
    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(201, ApiResponse<CustomerDto>.Created(result, "Customer created."));
    }
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var command = new UpdateCustomerCommand(id, request.FullName, request.Email, request.Phone, request.Company, request.Status);
        var result  = await mediator.Send(command);
        return Ok(ApiResponse<CustomerDto>.Ok(result, "Customer updated."));
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await mediator.Send(new DeleteCustomerCommand(id));
        return Ok(ApiResponse<bool>.Ok(true, "Customer deleted."));
    }
}
public record UpdateCustomerRequest(
    string FullName,
    string Email,
    string? Phone,
    string? Company,
    CustomerStatus Status);
