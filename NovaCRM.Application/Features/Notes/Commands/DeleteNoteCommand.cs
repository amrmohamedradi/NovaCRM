using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Notes.Commands;
public record DeleteNoteCommand(Guid Id) : IRequest<bool>;

public class DeleteNoteCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteNoteCommand, bool>
{
    public async Task<bool> Handle(DeleteNoteCommand request, CancellationToken ct)
    {
        var note = await context.Notes.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Note {request.Id} not found.");

        context.Notes.Remove(note);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
