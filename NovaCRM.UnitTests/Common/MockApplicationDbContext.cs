using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace NovaCRM.UnitTests.Common;

public class MockApplicationDbContext : DbContext, IApplicationDbContext
{
    public MockApplicationDbContext(DbContextOptions<MockApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<Note> Notes => Set<Note>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
