using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Deals.Commands;
public record UpdateDealCommand(
    Guid Id,
    string Title,
    decimal Value,
    DealStage Stage,
    DateTime? ExpectedCloseDate) : IRequest<DealDto>;

public class UpdateDealCommandHandler(IRepository<Deal> repo, IMapper mapper)
    : IRequestHandler<UpdateDealCommand, DealDto>
{
    public async Task<DealDto> Handle(UpdateDealCommand request, CancellationToken ct)
    {
        var deal = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Deal {request.Id} not found.");

        deal.Title             = request.Title;
        deal.Value             = request.Value;
        deal.Stage             = request.Stage;
        deal.ExpectedCloseDate = request.ExpectedCloseDate;

        repo.Update(deal);
        await repo.SaveChangesAsync();
        return mapper.Map<DealDto>(deal);
    }
}



