using Gathering.Domain.Sessions;

namespace Gathering.Application.Sessions.GetById;

public sealed record GetSessionByIdQueryResponse(
    Guid Id,
    Guid CommunityId,
    string Title,
    string Description,
    string? Image,
    string Speaker,
    DateTime Schedule,
    SessionState State);
