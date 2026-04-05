using MediatR;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Notes.Commands;
public record DeleteNoteCommand(Guid Id) : IRequest<bool>;

public class DeleteNoteCommandHandler(IRepository<Note> repo)
    : IRequestHandler<DeleteNoteCommand, bool>
{
    public async Task<bool> Handle(DeleteNoteCommand request, CancellationToken ct)
    {
        var note = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Note {request.Id} not found.");

        repo.Delete(note);
        await repo.SaveChangesAsync();
        return true;
    }
}



