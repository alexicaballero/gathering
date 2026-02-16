

using Gathering.SharedKernel;

namespace Gathering.Domain.Communities;

public sealed record CommunityCreatedDomainEvent(Guid CommunityId) : IDomainEvent;
