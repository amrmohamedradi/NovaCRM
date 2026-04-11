using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Notes.Queries;

public record GetNotesByCustomerIdQuery(Guid CustomerId) : IRequest<List<NoteDto>>;

public class GetNotesByCustomerIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetNotesByCustomerIdQuery, List<NoteDto>>
{
    public async Task<List<NoteDto>> Handle(GetNotesByCustomerIdQuery request, CancellationToken ct)
    {

        var notes = await context.Notes
            .AsNoTracking()
            .Where(n => n.CustomerId == request.CustomerId)
            .OrderByDescending(n => n.CreatedAt)
            .ProjectTo<NoteDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return notes;
    }
}
