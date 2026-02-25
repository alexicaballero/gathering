using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.GetByCommunity;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.GetActive;

public sealed class GetActiveSessionsQueryHandler : IQueryHandler<GetActiveSessionsQuery, IReadOnlyList<GetSessionsByCommunityQueryResponse>>
{
  private readonly ISessionRepository _sessionRepository;

  public GetActiveSessionsQueryHandler(ISessionRepository sessionRepository)
  {
    _sessionRepository = sessionRepository;
  }

  public async Task<Result<IReadOnlyList<GetSessionsByCommunityQueryResponse>>> HandleAsync(
      GetActiveSessionsQuery request,
      CancellationToken cancellationToken = default)
  {
    var sessions = await _sessionRepository.GetActiveSessionsAsync(cancellationToken);

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
