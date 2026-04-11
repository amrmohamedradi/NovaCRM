using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Activities.Queries;

public record GetActivitiesByCustomerIdQuery(Guid CustomerId) : IRequest<List<ActivityDto>>;

public class GetActivitiesByCustomerIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetActivitiesByCustomerIdQuery, List<ActivityDto>>
{
    public async Task<List<ActivityDto>> Handle(
        GetActivitiesByCustomerIdQuery request, CancellationToken ct)
    {

        var activities = await context.Activities
            .AsNoTracking()
            .Where(a => a.CustomerId == request.CustomerId)
            .OrderByDescending(a => a.DueDate)
            .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return activities;
    }
}
