using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Domain.Common;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;
using NovaCRM.Infrastructure.Data;

namespace NovaCRM.Infrastructure.Repositories;
public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> _set = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        
        if (typeof(T) == typeof(Customer))
        {
            return await context.Customers
                .Include(c => c.Contacts)
                .Include(c => c.Deals)
                .Include(c => c.Notes)
                .Include(c => c.Activities)
                .FirstOrDefaultAsync(c => c.Id == id) as T;
        }

        
        if (typeof(T) == typeof(Deal))
        {
            return await context.Deals
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(d => d.Id == id) as T;
        }

        return await _set.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        if (typeof(T) == typeof(Customer))
        {
            return (await context.Customers
                .Include(c => c.Contacts)
                .Include(c => c.Deals)
                .Include(c => c.Notes)
                .Include(c => c.Activities)
                .ToListAsync()) as IEnumerable<T> ?? Enumerable.Empty<T>();
        }

        if (typeof(T) == typeof(Deal))
        {
            return (await context.Deals
                .Include(d => d.Customer)
                .ToListAsync()) as IEnumerable<T> ?? Enumerable.Empty<T>();
        }

        return await _set.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        if (typeof(T) == typeof(Customer))
        {
            var expr = predicate as Expression<Func<Customer, bool>>;
            if (expr != null)
            {
                var result = await context.Customers
                    .Include(c => c.Contacts)
                    .Include(c => c.Deals)
                    .Include(c => c.Notes)
                    .Include(c => c.Activities)
                    .Where(expr)
                    .ToListAsync();
                return (result as IEnumerable<T>)!;
            }
        }

        if (typeof(T) == typeof(Deal))
        {
            var expr = predicate as Expression<Func<Deal, bool>>;
            if (expr != null)
            {
                var result = await context.Deals
                    .Include(d => d.Customer)
                    .Where(expr)
                    .ToListAsync();
                return (result as IEnumerable<T>)!;
            }
        }

        return await _set.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity) => await _set.AddAsync(entity);

    public void Update(T entity) => _set.Update(entity);

    public void Delete(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}



