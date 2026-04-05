using NovaCRM.Domain.Common;

namespace NovaCRM.Domain.Entities;

public class Contact : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Position { get; set; }

    
    public Customer Customer { get; set; } = null!;
}



