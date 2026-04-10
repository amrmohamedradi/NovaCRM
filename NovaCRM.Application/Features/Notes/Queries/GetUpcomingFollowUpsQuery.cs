using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Notes.Queries;

public record GetUpcomingFollowUpsQuery : IRequest<List<NoteDto>>;

public class GetUpcomingFollowUpsQueryHandler(IRepository<Note> repo, IMapper mapper)
    : IRequestHandler<GetUpcomingFollowUpsQuery, List<NoteDto>>
{
    public async Task<List<NoteDto>> Handle(GetUpcomingFollowUpsQuery request, CancellationToken ct)
    {
        var now    = DateTime.UtcNow;
        var cutoff = now.AddDays(7);

        var notes = await repo.ExecuteAsync(
            repo.Query()
                .Where(n =>
                    n.FollowUpDate.HasValue &&
                    n.FollowUpDate!.Value >= now &&
                    n.FollowUpDate.Value  <= cutoff)
                .OrderBy(n => n.FollowUpDate),
            ct);

        return mapper.Map<List<NoteDto>>(notes);
    }
}
