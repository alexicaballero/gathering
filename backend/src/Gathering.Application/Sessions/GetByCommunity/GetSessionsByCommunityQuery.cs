using Gathering.Application.Abstractions;

namespace Gathering.Application.Sessions.GetByCommunity;

public sealed record GetSessionsByCommunityQuery(Guid CommunityId) : IQuery<IReadOnlyList<GetSessionsByCommunityQueryResponse>>;
