using MediatR;
using NovaCRM.Application.Common;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NovaCRM.Application.Common.EventHandlers;

public class GenericDomainEventNotificationHandler<TDomainEvent>(IDomainEventDispatcher dispatcher)
    : INotificationHandler<DomainEventNotification<TDomainEvent>>
    where TDomainEvent : IDomainEvent
{
    public Task Handle(DomainEventNotification<TDomainEvent> notification, CancellationToken cancellationToken)
    {
        return dispatcher.DispatchToClientsAsync(notification.DomainEvent, cancellationToken);
    }
}
