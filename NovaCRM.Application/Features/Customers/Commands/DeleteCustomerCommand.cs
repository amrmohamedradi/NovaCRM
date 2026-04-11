using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Customers.Commands;
public record DeleteCustomerCommand(Guid Id) : IRequest<bool>;

public class DeleteCustomerCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await context.Customers.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Customer {request.Id} not found.");

        context.Customers.Remove(customer);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
