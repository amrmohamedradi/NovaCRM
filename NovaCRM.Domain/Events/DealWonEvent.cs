using NovaCRM.Domain.Common;

namespace NovaCRM.Domain.Events;

public class DealWonEvent : IDomainEvent
{
    public Guid DealId { get; }
    public decimal Value { get; }
    
    public DealWonEvent(Guid dealId, decimal value)
    {
        DealId = dealId;
        Value = value;
    }
}
