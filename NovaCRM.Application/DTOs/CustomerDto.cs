using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public CustomerStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
public class CustomerDetailDto : CustomerDto
{
    public List<ContactDto> Contacts { get; set; } = new();
    public List<DealDto> Deals { get; set; } = new();
    public List<NoteDto> Notes { get; set; } = new();
}



