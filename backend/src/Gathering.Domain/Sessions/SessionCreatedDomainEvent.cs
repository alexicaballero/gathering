using Gathering.SharedKernel;

namespace Gathering.Domain.Sessions;

public sealed record SessionCreatedDomainEvent(Guid SessionId) : IDomainEvent;