using Gathering.Application.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.Resources.GetBySession;

public sealed class GetSessionResourcesQueryHandler : IQueryHandler<GetSessionResourcesQuery, IReadOnlyList<GetSessionResourcesQueryResponse>>
{
  private readonly ISessionRepository _sessionRepository;

  public GetSessionResourcesQueryHandler(ISessionRepository sessionRepository)
  {
    _sessionRepository = sessionRepository;
  }

  public async Task<Result<IReadOnlyList<GetSessionResourcesQueryResponse>>> HandleAsync(GetSessionResourcesQuery request, CancellationToken cancellationToken = default)
  {
    var sessionExists = await _sessionRepository.ExistsAsync(request.SessionId, cancellationToken);
    if (!sessionExists)
    {
      return Result.Failure<IReadOnlyList<GetSessionResourcesQueryResponse>>(SessionError.NotFound);
    }

    var resources = await _sessionRepository.GetResourcesBySessionIdAsync(request.SessionId, cancellationToken);

    return resources.Select(r => new GetSessionResourcesQueryResponse(
        r.Id,
        r.SessionId,
        r.Type,
        r.Title,
        r.Url,
        r.Notes,
        r.CreatedAt,
        r.UpdatedAt)).ToList().AsReadOnly();
  }
}
