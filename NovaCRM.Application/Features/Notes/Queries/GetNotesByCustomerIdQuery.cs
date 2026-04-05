using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Notes.Queries;
public record GetNotesByCustomerIdQuery(Guid CustomerId) : IRequest<List<NoteDto>>;

public class GetNotesByCustomerIdQueryHandler(IRepository<Note> repo, IMapper mapper)
    : IRequestHandler<GetNotesByCustomerIdQuery, List<NoteDto>>
{
    public async Task<List<NoteDto>> Handle(GetNotesByCustomerIdQuery request, CancellationToken ct)
    {
        var notes = await repo.FindAsync(n => n.CustomerId == request.CustomerId);
        return mapper.Map<List<NoteDto>>(notes.OrderByDescending(n => n.CreatedAt).ToList());
    }
}



