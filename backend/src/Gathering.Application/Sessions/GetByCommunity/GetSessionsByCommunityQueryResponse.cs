using Gathering.Domain.Sessions;

namespace Gathering.Application.Sessions.GetByCommunity;

public sealed record GetSessionsByCommunityQueryResponse(
    Guid Id,
    Guid CommunityId,
    string Title,
    string Description,
    string? Image,
    string Speaker,
    DateTime Schedule,
    SessionState State);
