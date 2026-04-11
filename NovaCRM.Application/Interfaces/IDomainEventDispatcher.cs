using NovaCRM.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NovaCRM.Application.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchToClientsAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
