using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Deals.Queries;
public record GetDealByIdQuery(Guid Id) : IRequest<DealDto>;

public class GetDealByIdQueryHandler(IRepository<Deal> repo, IMapper mapper)
    : IRequestHandler<GetDealByIdQuery, DealDto>
{
    public async Task<DealDto> Handle(GetDealByIdQuery request, CancellationToken ct)
    {
        var deal = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Deal {request.Id} not found.");

        return mapper.Map<DealDto>(deal);
    }
}
