using Gathering.Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gathering.Infrastructure.Repositories;

internal sealed class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SessionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Session entity)
    {
        _dbContext.Sessions.Add(entity);
    }

    public void AddRange(IEnumerable<Session> entities)
    {
        _dbContext.Sessions.AddRange(entities);
    }

    public async Task<bool> AnyAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Session>> FindAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions.Where(predicate).ToListAsync(cancellationToken);
        return sessions.AsReadOnly();
    }

    public async Task<Session?> FirstOrDefaultAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<Session>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions.ToListAsync(cancellationToken);
        return sessions.AsReadOnly();
    }

    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Include(s => s.Resources)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public void Remove(Session entity)
    {
        _dbContext.Sessions.Remove(entity);
    }

    public void RemoveRange(IEnumerable<Session> entities)
    {
        _dbContext.Sessions.RemoveRange(entities);
    }

    public void Update(Session entity)
    {
        _dbContext.Sessions.Update(entity);
    }

    public void UpdateRange(IEnumerable<Session> entities)
    {
        _dbContext.Sessions.UpdateRange(entities);
    }

    public async Task<IReadOnlyList<Session>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.CommunityId == communityId)
            .ToListAsync(cancellationToken);

        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyList<Session>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.State == SessionState.Scheduled)
            .ToListAsync(cancellationToken);

        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyList<SessionResource>> GetResourcesBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var resources = await _dbContext.SessionResources
            .Where(r => r.SessionId == sessionId)
            .ToListAsync(cancellationToken);

        return resources.AsReadOnly();
    }
}
