using NovaCRM.Domain.Common;

namespace NovaCRM.Domain.Entities;

public class Attachment : BaseEntity
{
    public Guid   CustomerId   { get; set; }
    public Customer Customer   { get; set; } = null!;

    public string FileName     { get; set; } = string.Empty;

    public string StoredName   { get; set; } = string.Empty;

    public string ContentType  { get; set; } = string.Empty;
    public long   SizeBytes    { get; set; }
}
