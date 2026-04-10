using System.Linq.Expressions;
using NovaCRM.Domain.Common;

namespace NovaCRM.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{

    Task<T?> GetByIdAsync(Guid id);

    IQueryable<T> Query();

    Task<List<T>> ExecuteAsync(IQueryable<T> query, CancellationToken ct = default);

    Task<int> CountAsync(IQueryable<T> query, CancellationToken ct = default);

    Task<int> CountWhereAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default);

    Task<decimal> SumDecimalAsync(
        Expression<Func<T, decimal>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}
