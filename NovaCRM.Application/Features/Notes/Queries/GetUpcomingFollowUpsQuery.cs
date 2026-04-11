using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Notes.Queries;

public record GetUpcomingFollowUpsQuery : IRequest<List<NoteDto>>;

public class GetUpcomingFollowUpsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetUpcomingFollowUpsQuery, List<NoteDto>>
{
    public async Task<List<NoteDto>> Handle(GetUpcomingFollowUpsQuery request, CancellationToken ct)
    {
        var now    = DateTime.UtcNow;
        var cutoff = now.AddDays(7);

        var notes = await context.Notes
            .AsNoTracking()
            .Where(n =>
                n.FollowUpDate.HasValue &&
                n.FollowUpDate!.Value >= now &&
                n.FollowUpDate.Value  <= cutoff)
            .OrderBy(n => n.FollowUpDate)
            .ProjectTo<NoteDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return notes;
    }
}
