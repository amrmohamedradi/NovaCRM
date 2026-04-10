using MediatR;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Deals.Commands;
public record DeleteDealCommand(Guid Id) : IRequest<bool>;

public class DeleteDealCommandHandler(IRepository<Deal> repo)
    : IRequestHandler<DeleteDealCommand, bool>
{
    public async Task<bool> Handle(DeleteDealCommand request, CancellationToken ct)
    {
        var deal = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Deal {request.Id} not found.");

        repo.Delete(deal);
        await repo.SaveChangesAsync();
        return true;
    }
}
