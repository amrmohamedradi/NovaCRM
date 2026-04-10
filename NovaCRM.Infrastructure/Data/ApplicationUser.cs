using Microsoft.AspNetCore.Identity;

namespace NovaCRM.Infrastructure.Data;
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
