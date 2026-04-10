using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Notes.Commands;
public record CreateNoteCommand(
    Guid CustomerId,
    string Content,
    DateTime? FollowUpDate) : IRequest<NoteDto>;

public class CreateNoteCommandHandler(
    IRepository<Note> repo,
    IRepository<Customer> customerRepo,
    IMapper mapper)
    : IRequestHandler<CreateNoteCommand, NoteDto>
{
    public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken ct)
    {
        _ = await customerRepo.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var note = new Note
        {
            CustomerId   = request.CustomerId,
            Content      = request.Content,
            FollowUpDate = request.FollowUpDate
        };

        await repo.AddAsync(note);
        await repo.SaveChangesAsync();
        return mapper.Map<NoteDto>(note);
    }
}
