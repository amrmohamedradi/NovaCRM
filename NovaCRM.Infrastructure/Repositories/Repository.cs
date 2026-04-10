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
                .Include(c => c.Attachments)
                .AsSplitQuery()
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

    public IQueryable<T> Query()
    {
        if (typeof(T) == typeof(Deal))
        {

            return (context.Deals
                .AsNoTracking()
                .Include(d => d.Customer) as IQueryable<T>)!;
        }

        return _set.AsNoTracking();
    }

    public Task<List<T>> ExecuteAsync(IQueryable<T> query, CancellationToken ct = default)
        => query.ToListAsync(ct);

    public Task<int> CountAsync(IQueryable<T> query, CancellationToken ct = default)
        => query.CountAsync(ct);

    public Task<int> CountWhereAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        var q = _set.AsNoTracking();
        return predicate is null ? q.CountAsync(ct) : q.Where(predicate).CountAsync(ct);
    }

    public Task<decimal> SumDecimalAsync(
        Expression<Func<T, decimal>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        var q = _set.AsNoTracking();
        if (predicate is not null) q = q.Where(predicate);
        return q.SumAsync(selector, ct);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        if (typeof(T) == typeof(Customer))
        {
            return (await context.Customers
                .AsNoTracking()
                .Include(c => c.Contacts)
                .Include(c => c.Deals)
                .Include(c => c.Notes)
                .Include(c => c.Activities)
                .AsSplitQuery()
                .ToListAsync()) as IEnumerable<T> ?? Enumerable.Empty<T>();
        }

        if (typeof(T) == typeof(Deal))
        {
            return (await context.Deals
                .AsNoTracking()
                .Include(d => d.Customer)
                .ToListAsync()) as IEnumerable<T> ?? Enumerable.Empty<T>();
        }

        return await _set.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        if (typeof(T) == typeof(Customer))
        {
            var expr = predicate as Expression<Func<Customer, bool>>;
            if (expr is not null)
            {
                var result = await context.Customers
                    .AsNoTracking()
                    .Include(c => c.Contacts)
                    .Include(c => c.Deals)
                    .Include(c => c.Notes)
                    .Include(c => c.Activities)
                    .AsSplitQuery()
                    .Where(expr)
                    .ToListAsync();
                return (result as IEnumerable<T>)!;
            }
        }

        if (typeof(T) == typeof(Deal))
        {
            var expr = predicate as Expression<Func<Deal, bool>>;
            if (expr is not null)
            {
                var result = await context.Deals
                    .AsNoTracking()
                    .Include(d => d.Customer)
                    .Where(expr)
                    .ToListAsync();
                return (result as IEnumerable<T>)!;
            }
        }

        return await _set.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity) => await _set.AddAsync(entity);
    public void Update(T entity)         => _set.Update(entity);
    public void Delete(T entity)         => _set.Remove(entity);
    public Task<int> SaveChangesAsync()  => context.SaveChangesAsync();
}
