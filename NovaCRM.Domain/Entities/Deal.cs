using NovaCRM.Domain.Common;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Events;

namespace NovaCRM.Domain.Entities;

public class Deal : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DealStage Stage { get; set; } = DealStage.New;
    public DateTime? ExpectedCloseDate { get; set; }

    public Customer Customer { get; set; } = null!;
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public string? AssignedToUserId { get; set; }

    public void MarkAsWon()
    {
        if (Stage != DealStage.Won)
        {
            Stage = DealStage.Won;
            AddDomainEvent(new DealWonEvent(Id, Value));
        }
    }
}
