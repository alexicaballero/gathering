using Gathering.Domain.Abstractions;

namespace Gathering.Domain.Sessions;

public interface ISessionRepository : IRepository<Session>
{
    Task<IReadOnlyList<Session>> GetByCommunityIdAsync(
        Guid communityId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Session>> GetActiveSessionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SessionResource>> GetResourcesBySessionIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);

    void AddResource(SessionResource resource);
}