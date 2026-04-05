using NovaCRM.Domain.Common;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Domain.Entities;

public class Activity : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid? DealId { get; set; }
    public ActivityType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsDone { get; set; }

    
    public Customer Customer { get; set; } = null!;
    public Deal? Deal { get; set; }
}



