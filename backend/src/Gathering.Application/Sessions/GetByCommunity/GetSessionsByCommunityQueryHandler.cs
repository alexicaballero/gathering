using Gathering.Application.Abstractions;
using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.GetByCommunity;

public sealed class GetSessionsByCommunityQueryHandler : IQueryHandler<GetSessionsByCommunityQuery, IReadOnlyList<GetSessionsByCommunityQueryResponse>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICommunityRepository _communityRepository;

    public GetSessionsByCommunityQueryHandler(
        ISessionRepository sessionRepository,
        ICommunityRepository communityRepository)
    {
        _sessionRepository = sessionRepository;
        _communityRepository = communityRepository;
    }

    public async Task<Result<IReadOnlyList<GetSessionsByCommunityQueryResponse>>> HandleAsync(GetSessionsByCommunityQuery request, CancellationToken cancellationToken = default)
    {
        bool communityExists = await _communityRepository.ExistsAsync(request.CommunityId, cancellationToken);
        if (!communityExists)
        {
            return Result.Failure<IReadOnlyList<GetSessionsByCommunityQueryResponse>>(CommunityError.NotFound);
        }

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