using Microsoft.EntityFrameworkCore;
using NovaCRM.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace NovaCRM.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Activity> Activities { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Deal> Deals { get; }
    DbSet<Note> Notes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
