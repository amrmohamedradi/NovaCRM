using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.DTOs;

public class ActivityDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? DealId { get; set; }
    public ActivityType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}



