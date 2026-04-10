using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db          = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.EnsureCreatedAsync();

        string[] roles = ["Admin", "Sales", "Viewer"];
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        await EnsureUser(userManager, "Admin User",  "admin@novacrm.com", "Admin@123",  "Admin");
        await EnsureUser(userManager, "Sales User",  "sales@novacrm.com", "Sales@123",  "Sales");
        await EnsureUser(userManager, "Viewer User", "viewer@novacrm.com","Viewer@123", "Viewer");

        if (await db.Customers.AnyAsync()) return;

        var customers = new List<Customer>
        {
            new() { FullName = "Alice Johnson",  Email = "alice@acme.com",    Phone = "555-1001", Company = "Acme Corp",      Status = CustomerStatus.Active   },
            new() { FullName = "Bob Smith",       Email = "bob@techwave.io",   Phone = "555-1002", Company = "TechWave",       Status = CustomerStatus.Lead     },
            new() { FullName = "Carol Williams",  Email = "carol@bluesky.net", Phone = "555-1003", Company = "BlueSky Ltd",    Status = CustomerStatus.Active   },
            new() { FullName = "David Brown",     Email = "david@nexgen.co",   Phone = "555-1004", Company = "NexGen",         Status = CustomerStatus.Inactive },
            new() { FullName = "Eve Martinez",    Email = "eve@startupz.com",  Phone = "555-1005", Company = "StartupZ",       Status = CustomerStatus.Lead     }
        };
        await db.Customers.AddRangeAsync(customers);
        await db.SaveChangesAsync();

        var contacts = customers.SelectMany(c => new[]
        {
            new Contact { CustomerId = c.Id, FullName = $"{c.FullName} - Primary",   Email = $"primary.{c.Email}",   Phone = c.Phone,   Position = "CEO"       },
            new Contact { CustomerId = c.Id, FullName = $"{c.FullName} - Secondary", Email = $"secondary.{c.Email}", Phone = c.Phone,   Position = "CTO"       }
        }).ToList();
        await db.Contacts.AddRangeAsync(contacts);

        var deals = new List<Deal>
        {
            new() { CustomerId = customers[0].Id, Title = "Enterprise License",   Value = 45000, Stage = DealStage.Won,         ExpectedCloseDate = DateTime.UtcNow.AddDays(-10) },
            new() { CustomerId = customers[0].Id, Title = "Support Contract",     Value = 12000, Stage = DealStage.Negotiation, ExpectedCloseDate = DateTime.UtcNow.AddDays(15)  },
            new() { CustomerId = customers[1].Id, Title = "Cloud Migration",      Value = 78000, Stage = DealStage.Proposal,    ExpectedCloseDate = DateTime.UtcNow.AddDays(30)  },
            new() { CustomerId = customers[1].Id, Title = "API Integration",      Value = 9500,  Stage = DealStage.Qualified,   ExpectedCloseDate = DateTime.UtcNow.AddDays(45)  },
            new() { CustomerId = customers[2].Id, Title = "Annual Subscription",  Value = 24000, Stage = DealStage.Won,         ExpectedCloseDate = DateTime.UtcNow.AddDays(-5)  },
            new() { CustomerId = customers[2].Id, Title = "Hardware Procurement", Value = 32000, Stage = DealStage.New,         ExpectedCloseDate = DateTime.UtcNow.AddDays(60)  },
            new() { CustomerId = customers[3].Id, Title = "Legacy Upgrade",       Value = 55000, Stage = DealStage.Lost,        ExpectedCloseDate = DateTime.UtcNow.AddDays(-20) },
            new() { CustomerId = customers[4].Id, Title = "Startup Package",      Value = 6500,  Stage = DealStage.Qualified,   ExpectedCloseDate = DateTime.UtcNow.AddDays(20)  }
        };
        await db.Deals.AddRangeAsync(deals);

        var notes = new List<Note>
        {
            new() { CustomerId = customers[0].Id, Content = "Discussed expansion plans. Very interested in our enterprise tier.",  FollowUpDate = DateTime.UtcNow.AddDays(2)  },
            new() { CustomerId = customers[0].Id, Content = "Alice requested a demo of the new reporting module.",                  FollowUpDate = null                         },
            new() { CustomerId = customers[1].Id, Content = "Bob is evaluating three vendors. Need to send competitive analysis.", FollowUpDate = DateTime.UtcNow.AddDays(4)  },
            new() { CustomerId = customers[1].Id, Content = "Initial discovery call completed. Budget confirmed.",                  FollowUpDate = null                         },
            new() { CustomerId = customers[2].Id, Content = "Carol wants SLA guarantees in the contract.",                         FollowUpDate = DateTime.UtcNow.AddDays(6)  },
            new() { CustomerId = customers[2].Id, Content = "Annual review scheduled for next quarter.",                           FollowUpDate = null                         },
            new() { CustomerId = customers[3].Id, Content = "David's company hit budget freeze. Opportunity on hold.",             FollowUpDate = null                         },
            new() { CustomerId = customers[4].Id, Content = "Eve is a startup — price sensitive. Offered discount.",               FollowUpDate = DateTime.UtcNow.AddDays(3)  },
            new() { CustomerId = customers[4].Id, Content = "Sent onboarding checklist.",                                          FollowUpDate = null                         },
            new() { CustomerId = customers[3].Id, Content = "Reached out to reopen conversation after Q2.",                        FollowUpDate = DateTime.UtcNow.AddDays(7)  }
        };
        await db.Notes.AddRangeAsync(notes);

        var activities = new List<Activity>
        {
            new() { CustomerId = customers[0].Id, DealId = deals[0].Id, Type = ActivityType.Call,    Description = "Closing call for Enterprise License deal.",  DueDate = DateTime.UtcNow.AddDays(-10), IsDone = true  },
            new() { CustomerId = customers[0].Id, DealId = deals[1].Id, Type = ActivityType.Email,   Description = "Send revised support contract proposal.",     DueDate = DateTime.UtcNow.AddDays(2),  IsDone = false },
            new() { CustomerId = customers[1].Id, DealId = deals[2].Id, Type = ActivityType.Meeting, Description = "Cloud migration scoping workshop.",           DueDate = DateTime.UtcNow.AddDays(5),  IsDone = false },
            new() { CustomerId = customers[1].Id, DealId = null,        Type = ActivityType.Task,    Description = "Prepare competitive analysis document.",      DueDate = DateTime.UtcNow.AddDays(3),  IsDone = false },
            new() { CustomerId = customers[2].Id, DealId = deals[4].Id, Type = ActivityType.Call,    Description = "Annual renewal call — confirm satisfaction.", DueDate = DateTime.UtcNow.AddDays(-5), IsDone = true  },
            new() { CustomerId = customers[2].Id, DealId = deals[5].Id, Type = ActivityType.Meeting, Description = "Hardware requirements review meeting.",       DueDate = DateTime.UtcNow.AddDays(10), IsDone = false },
            new() { CustomerId = customers[3].Id, DealId = null,        Type = ActivityType.Email,   Description = "Re-engagement email after budget freeze.",    DueDate = DateTime.UtcNow.AddDays(1),  IsDone = false },
            new() { CustomerId = customers[4].Id, DealId = deals[7].Id, Type = ActivityType.Call,    Description = "Onboarding kickoff call.",                    DueDate = DateTime.UtcNow.AddDays(4),  IsDone = false },
            new() { CustomerId = customers[4].Id, DealId = null,        Type = ActivityType.Task,    Description = "Send welcome pack and documentation.",        DueDate = DateTime.UtcNow.AddDays(1),  IsDone = true  },
            new() { CustomerId = customers[0].Id, DealId = null,        Type = ActivityType.Meeting, Description = "Quarterly business review.",                  DueDate = DateTime.UtcNow.AddDays(14), IsDone = false }
        };
        await db.Activities.AddRangeAsync(activities);

        await db.SaveChangesAsync();
    }

    private static async Task EnsureUser(
        UserManager<ApplicationUser> userManager,
        string fullName, string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) != null) return;

        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);
    }
}
