using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Deals.Commands;
public record CreateDealCommand(
    Guid CustomerId,
    string Title,
    decimal Value,
    DealStage Stage,
    DateTime? ExpectedCloseDate) : IRequest<DealDto>;

public class CreateDealCommandHandler(
    IRepository<Deal> repo,
    IRepository<Customer> customerRepo,
    IMapper mapper)
    : IRequestHandler<CreateDealCommand, DealDto>
{
    public async Task<DealDto> Handle(CreateDealCommand request, CancellationToken ct)
    {
        _ = await customerRepo.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var deal = new Deal
        {
            CustomerId        = request.CustomerId,
            Title             = request.Title,
            Value             = request.Value,
            Stage             = request.Stage,
            ExpectedCloseDate = request.ExpectedCloseDate
        };

        await repo.AddAsync(deal);
        await repo.SaveChangesAsync();
        return mapper.Map<DealDto>(deal);
    }
}
