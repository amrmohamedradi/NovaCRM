using NovaCRM.Domain.Common;

namespace NovaCRM.Domain.Entities;

public class Note : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime? FollowUpDate { get; set; }

    
    public Customer Customer { get; set; } = null!;
}



