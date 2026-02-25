using Gathering.Domain.Communities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gathering.Infrastructure.Repositories;

internal sealed class CommunityRepository : ICommunityRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CommunityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Community entity)
    {
        _dbContext.Communities.Add(entity);
    }

    public void AddRange(IEnumerable<Community> entities)
    {
        _dbContext.Communities.AddRange(entities);
    }

    public async Task<bool> AnyAsync(Expression<Func<Community, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Community, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities.CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Community>> FindAsync(Expression<Func<Community, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var communities = await _dbContext.Communities
            .Where(predicate)
            .ToListAsync(cancellationToken);

        return communities.AsReadOnly();
    }

    public async Task<Community?> FirstOrDefaultAsync(Expression<Func<Community, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Community>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var communities = await _dbContext.Communities
            .Include(c => c.Sessions)
            .ToListAsync(cancellationToken);
        return communities.AsReadOnly();
    }

    public async Task<Community?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Communities
            .Include(c => c.Sessions)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public void Remove(Community entity)
    {
        _dbContext.Communities.Remove(entity);
    }

    public void RemoveRange(IEnumerable<Community> entities)
    {
        _dbContext.Communities.RemoveRange(entities);
    }

    public void Update(Community entity)
    {
        _dbContext.Communities.Update(entity);
    }

    public void UpdateRange(IEnumerable<Community> entities)
    {
        _dbContext.Communities.UpdateRange(entities);
    }
}
