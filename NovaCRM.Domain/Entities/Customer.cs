using NovaCRM.Domain.Common;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public CustomerStatus Status { get; set; } = CustomerStatus.Lead;
    public string? AssignedToUserId { get; set; }

    public ICollection<Contact>    Contacts    { get; set; } = new List<Contact>();
    public ICollection<Deal>       Deals       { get; set; } = new List<Deal>();
    public ICollection<Note>       Notes       { get; set; } = new List<Note>();
    public ICollection<Activity>   Activities  { get; set; } = new List<Activity>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
