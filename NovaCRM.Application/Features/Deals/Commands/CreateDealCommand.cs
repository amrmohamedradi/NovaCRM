using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Features.Deals.Commands;
public record CreateDealCommand(
    Guid CustomerId,
    string Title,
    decimal Value,
    DealStage Stage,
    DateTime? ExpectedCloseDate) : IRequest<DealDto>;

public class CreateDealCommandHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateDealCommand, DealDto>
{
    public async Task<DealDto> Handle(CreateDealCommand request, CancellationToken ct)
    {
        _ = await context.Customers.FindAsync(new object[] { request.CustomerId }, ct)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var deal = new Deal
        {
            CustomerId        = request.CustomerId,
            Title             = request.Title,
            Value             = request.Value,
            Stage             = request.Stage,
            ExpectedCloseDate = request.ExpectedCloseDate
        };

        context.Deals.Add(deal);
        await context.SaveChangesAsync(ct);
        return mapper.Map<DealDto>(deal);
    }
}
