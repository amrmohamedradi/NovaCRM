using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Notes.Commands;
public record CreateNoteCommand(
    Guid CustomerId,
    string Content,
    DateTime? FollowUpDate) : IRequest<NoteDto>;

public class CreateNoteCommandHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateNoteCommand, NoteDto>
{
    public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken ct)
    {
        _ = await context.Customers.FindAsync(new object[] { request.CustomerId }, ct)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var note = new Note
        {
            CustomerId   = request.CustomerId,
            Content      = request.Content,
            FollowUpDate = request.FollowUpDate
        };

        context.Notes.Add(note);
        await context.SaveChangesAsync(ct);
        return mapper.Map<NoteDto>(note);
    }
}
