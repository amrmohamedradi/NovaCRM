namespace NovaCRM.Application.DTOs;

public class AttachmentDto
{
    public Guid   Id          { get; set; }
    public Guid   CustomerId  { get; set; }
    public string FileName    { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long   SizeBytes   { get; set; }

    public string SizeFormatted { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public string?  CreatedBy { get; set; }
}
