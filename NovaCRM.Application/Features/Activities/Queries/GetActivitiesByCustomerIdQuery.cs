using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Activities.Queries;

public record GetActivitiesByCustomerIdQuery(Guid CustomerId) : IRequest<List<ActivityDto>>;

public class GetActivitiesByCustomerIdQueryHandler(IRepository<Activity> repo, IMapper mapper)
    : IRequestHandler<GetActivitiesByCustomerIdQuery, List<ActivityDto>>
{
    public async Task<List<ActivityDto>> Handle(
        GetActivitiesByCustomerIdQuery request, CancellationToken ct)
    {

        var activities = await repo.ExecuteAsync(
            repo.Query()
                .Where(a => a.CustomerId == request.CustomerId)
                .OrderByDescending(a => a.DueDate),
            ct);

        return mapper.Map<List<ActivityDto>>(activities);
    }
}
