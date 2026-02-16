using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Shared;

namespace Gathering.Application.Communities.GetAll;

public record GetCommunitiesQuery() : IQuery<IReadOnlyList<CommunityResponse>>;
