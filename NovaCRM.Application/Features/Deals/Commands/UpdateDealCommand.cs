using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Features.Deals.Commands;
public record UpdateDealCommand(
    Guid Id,
    string Title,
    decimal Value,
    DealStage Stage,
    DateTime? ExpectedCloseDate) : IRequest<DealDto>;

public class UpdateDealCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateDealCommand, DealDto>
{
    public async Task<DealDto> Handle(UpdateDealCommand request, CancellationToken ct)
    {
        var deal = await context.Deals.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Deal {request.Id} not found.");

        deal.Title             = request.Title;
        deal.Value             = request.Value;
        deal.Stage             = request.Stage;
        deal.ExpectedCloseDate = request.ExpectedCloseDate;

        context.Deals.Update(deal);
        await context.SaveChangesAsync(ct);
        return mapper.Map<DealDto>(deal);
    }
}
