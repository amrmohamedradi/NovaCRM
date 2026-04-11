using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Deals.Commands;
public record DeleteDealCommand(Guid Id) : IRequest<bool>;

public class DeleteDealCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteDealCommand, bool>
{
    public async Task<bool> Handle(DeleteDealCommand request, CancellationToken ct)
    {
        var deal = await context.Deals.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Deal {request.Id} not found.");

        context.Deals.Remove(deal);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
