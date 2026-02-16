using Gathering.SharedKernel;
using System.Linq.Expressions;

namespace Gathering.Domain.Abstractions;

public interface IRepository<T> where T : Entity
{
    // Queries
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Count
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Commands (estos NO deberían ser async - son solo operaciones en memoria)
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
