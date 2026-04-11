using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Deals.Queries;
public record GetDealByIdQuery(Guid Id) : IRequest<DealDto>;

public class GetDealByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetDealByIdQuery, DealDto>
{
    public async Task<DealDto> Handle(GetDealByIdQuery request, CancellationToken ct)
    {
        var deal = await context.Deals
            .AsNoTracking()
            .Where(d => d.Id == request.Id)
            .ProjectTo<DealDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"Deal {request.Id} not found.");

        return deal;
    }
}
