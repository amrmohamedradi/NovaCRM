using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Attachments.Commands;
using NovaCRM.Application.Features.Attachments.Queries;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.API.Controllers;

[ApiController]
[Authorize]
public class AttachmentsController(
    IMediator mediator,
    IFileStorageService fileStorage,
    IApplicationDbContext context) : ControllerBase
{
    private static readonly HashSet<string> AllowedTypes =
    [
        "application/pdf",
        "image/png", "image/jpeg", "image/gif",
        "text/plain",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ];

    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    [HttpPost("api/customers/{customerId:guid}/attachments")]
    [Authorize(Roles = "Admin,Sales")]
    [ProducesResponseType(typeof(ApiResponse<AttachmentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ApiResponse<AttachmentDto>>> Upload(
        Guid customerId,
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<AttachmentDto>.Fail("No file provided."));

        if (file.Length > MaxFileSizeBytes)
            return BadRequest(ApiResponse<AttachmentDto>.Fail("File exceeds 10 MB limit."));

        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (!AllowedTypes.Contains(contentType))
            return BadRequest(ApiResponse<AttachmentDto>.Fail(
                "File type not allowed. Permitted: PDF, PNG, JPEG, GIF, TXT, DOC, DOCX, XLS, XLSX."));

        await using var stream = file.OpenReadStream();

        var command = new UploadAttachmentCommand(
            customerId,
            file.FileName,
            contentType,
            file.Length,
            stream);

        var dto = await mediator.Send(command, ct);
        return StatusCode(201, ApiResponse<AttachmentDto>.Created(dto, "Attachment uploaded."));
    }

    [HttpGet("api/customers/{customerId:guid}/attachments")]
    [ProducesResponseType(typeof(ApiResponse<List<AttachmentDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<AttachmentDto>>>> GetByCustomer(
        Guid customerId,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetAttachmentsByCustomerIdQuery(customerId), ct);
        return Ok(ApiResponse<List<AttachmentDto>>.Ok(result));
    }

    [HttpGet("api/attachments/{id:guid}/download")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var attachment = await context.Attachments.FindAsync(new object[] { id });
        if (attachment is null)
            return NotFound(ApiResponse<object>.Fail($"Attachment {id} not found."));

        var file = await fileStorage.GetAsync(attachment.CustomerId, attachment.StoredName, ct);
        if (file is null)
            return NotFound(ApiResponse<object>.Fail("Attachment file not found on disk."));

        var (stream, contentType) = file.Value;
        return File(stream, contentType, attachment.FileName);
    }

    [HttpDelete("api/attachments/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAttachmentCommand(id), ct);
        return Ok(ApiResponse<object>.Ok(null!, "Attachment deleted."));
    }
}
