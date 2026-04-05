using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Domain.Common;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Customer>  Customers  => Set<Customer>();
    public DbSet<Contact>   Contacts   => Set<Contact>();
    public DbSet<Deal>      Deals      => Set<Deal>();
    public DbSet<Note>      Notes      => Set<Note>();
    public DbSet<Activity>  Activities => Set<Activity>();

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
    }
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}



