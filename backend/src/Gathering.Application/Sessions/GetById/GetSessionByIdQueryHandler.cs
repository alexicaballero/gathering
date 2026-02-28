using Gathering.Application.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.GetById;

public sealed class GetSessionByIdQueryHandler : IQueryHandler<GetSessionByIdQuery, GetSessionByIdQueryResponse>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionByIdQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<GetSessionByIdQueryResponse>> HandleAsync(GetSessionByIdQuery request, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null)
        {
            return Result.Failure<GetSessionByIdQueryResponse>(SessionError.NotFound);
        }

        return new GetSessionByIdQueryResponse(
            session.Id,
            session.CommunityId,
            session.Title,
            session.Description,
            session.Image,
            session.Speaker,
            session.ScheduledAt,
            session.Status);
    }
}
