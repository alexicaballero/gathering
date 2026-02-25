using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.GetByCommunity;

namespace Gathering.Application.Sessions.GetActive;

public sealed record GetActiveSessionsQuery : IQuery<IReadOnlyList<GetSessionsByCommunityQueryResponse>>;
