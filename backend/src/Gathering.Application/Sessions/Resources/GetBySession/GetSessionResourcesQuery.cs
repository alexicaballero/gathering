using Gathering.Application.Abstractions;

namespace Gathering.Application.Sessions.Resources.GetBySession;

public sealed record GetSessionResourcesQuery(Guid SessionId) : IQuery<IReadOnlyList<GetSessionResourcesQueryResponse>>;
