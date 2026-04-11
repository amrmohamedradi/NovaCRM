using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Activities.Queries;

public record GetActivitiesByDealIdQuery(Guid DealId) : IRequest<List<ActivityDto>>;

public class GetActivitiesByDealIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetActivitiesByDealIdQuery, List<ActivityDto>>
{
    public async Task<List<ActivityDto>> Handle(
        GetActivitiesByDealIdQuery request, CancellationToken ct)
    {

        var activities = await context.Activities
            .AsNoTracking()
            .Where(a => a.DealId == request.DealId)
            .OrderByDescending(a => a.DueDate)
            .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return activities;
    }
}
