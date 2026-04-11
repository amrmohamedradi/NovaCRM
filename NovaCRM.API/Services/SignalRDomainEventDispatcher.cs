using Microsoft.AspNetCore.SignalR;
using NovaCRM.API.Hubs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NovaCRM.API.Services;

public class SignalRDomainEventDispatcher(IHubContext<DomainEventHub> hubContext) : IDomainEventDispatcher
{
    public Task DispatchToClientsAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventName = domainEvent.GetType().Name;
        return hubContext.Clients.All.SendAsync("ReceiveDomainEvent", eventName, domainEvent, cancellationToken);
    }
}
