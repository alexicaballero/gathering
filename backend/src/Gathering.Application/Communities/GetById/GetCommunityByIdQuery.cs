using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Shared;

namespace Gathering.Application.Communities.GetById;

public sealed record GetCommunityByIdQuery(Guid CommunityId) : IQuery<CommunityResponse>;
