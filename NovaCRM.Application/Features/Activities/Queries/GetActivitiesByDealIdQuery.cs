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
    public async Task<List<ActivityDto>> Handle(
        GetActivitiesByDealIdQuery request, CancellationToken ct)
    {

        var activities = await repo.ExecuteAsync(
            repo.Query()
                .Where(a => a.DealId == request.DealId)
                .OrderByDescending(a => a.DueDate),
            ct);

        return mapper.Map<List<ActivityDto>>(activities);
    }
}
