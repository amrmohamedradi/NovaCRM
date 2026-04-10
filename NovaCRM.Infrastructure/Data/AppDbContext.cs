using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Common;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentUserService? _currentUser;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService? currentUser = null)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Customer>   Customers   => Set<Customer>();
    public DbSet<Contact>    Contacts    => Set<Contact>();
    public DbSet<Deal>       Deals       => Set<Deal>();
    public DbSet<Note>       Notes       => Set<Note>();
    public DbSet<Activity>   Activities  => Set<Activity>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Customer>()
            .HasMany(c => c.Contacts)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Customer>()
            .HasMany(c => c.Deals)
            .WithOne(d => d.Customer)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Customer>()
            .HasMany(c => c.Notes)
            .WithOne(n => n.Customer)
            .HasForeignKey(n => n.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Customer>()
            .HasMany(c => c.Activities)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Deal>()
            .HasMany(d => d.Activities)
            .WithOne(a => a.Deal)
            .HasForeignKey(a => a.DealId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Deal>()
            .Property(d => d.Value)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Customer>()
            .HasMany(c => c.Attachments)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Customer>()  .HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Contact>()   .HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Deal>()      .HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Note>()      .HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Activity>()  .HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Attachment>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {

        var actor = (_currentUser?.IsAuthenticated == true)
            ? (_currentUser.Email ?? _currentUser.UserId ?? "system")
            : "system";

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    entry.Entity.CreatedBy = actor;
                    entry.Entity.UpdatedBy = actor;
                    break;

                case EntityState.Modified:

                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = actor;
                    break;

                case EntityState.Deleted:

                    entry.State            = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = actor;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = actor;
                    break;
            }
        }

        return base.SaveChangesAsync(ct);
    }
}
