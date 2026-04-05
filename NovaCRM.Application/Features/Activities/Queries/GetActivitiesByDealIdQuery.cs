using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Activities.Queries;
public record GetActivitiesByDealIdQuery(Guid DealId) : IRequest<List<ActivityDto>>;

public class GetActivitiesByDealIdQueryHandler(IRepository<Activity> repo, IMapper mapper)
    : IRequestHandler<GetActivitiesByDealIdQuery, List<ActivityDto>>
{
    public async Task<List<ActivityDto>> Handle(GetActivitiesByDealIdQuery request, CancellationToken ct)
    {
        var activities = await repo.FindAsync(a => a.DealId == request.DealId);
        return mapper.Map<List<ActivityDto>>(activities.OrderByDescending(a => a.DueDate).ToList());
    }
}



