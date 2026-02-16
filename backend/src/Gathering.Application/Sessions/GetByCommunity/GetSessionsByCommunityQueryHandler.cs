using Gathering.Application.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.GetByCommunity;

public sealed class GetSessionsByCommunityQueryHandler : IQueryHandler<GetSessionsByCommunityQuery, IReadOnlyList<GetSessionsByCommunityQueryResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionsByCommunityQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<IReadOnlyList<GetSessionsByCommunityQueryResponse>>> HandleAsync(GetSessionsByCommunityQuery request, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionRepository.GetByCommunityIdAsync(request.CommunityId, cancellationToken);

        return sessions.Select(s => new GetSessionsByCommunityQueryResponse(
            s.Id,
            s.CommunityId,
            s.Title,
            s.Description,
            s.Image,
            s.Speaker,
            s.Schedule,
            s.State)).ToList().AsReadOnly();
    }
}