using MediatR;
using Microsoft.Extensions.Logging;
using NovaCRM.Application.Common;
using NovaCRM.Domain.Events;

namespace NovaCRM.Application.Features.Deals.EventHandlers;

public class SendEmailOnDealWonHandler(ILogger<SendEmailOnDealWonHandler> logger) 
    : INotificationHandler<DomainEventNotification<DealWonEvent>>
{
    public Task Handle(DomainEventNotification<DealWonEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        logger.LogInformation("Domain Event Received: Deal {DealId} was won with value {Value:C}. Sending celebratory email!", 
            domainEvent.DealId, domainEvent.Value);

        // Here we would use IEmailService to send actual email notifications!
        
        return Task.CompletedTask;
    }
}
