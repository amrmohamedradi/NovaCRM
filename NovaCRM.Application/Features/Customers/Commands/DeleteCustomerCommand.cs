using MediatR;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Customers.Commands;
public record DeleteCustomerCommand(Guid Id) : IRequest<bool>;

public class DeleteCustomerCommandHandler(IRepository<Customer> repo)
    : IRequestHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Customer {request.Id} not found.");

        repo.Delete(customer);
        await repo.SaveChangesAsync();
        return true;
    }
}



