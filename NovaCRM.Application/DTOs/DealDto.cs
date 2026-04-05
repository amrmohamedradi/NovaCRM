using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.DTOs;

public class DealDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DealStage Stage { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
public class DealPipelineDto
{
    public DealStage Stage { get; set; }
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
}



