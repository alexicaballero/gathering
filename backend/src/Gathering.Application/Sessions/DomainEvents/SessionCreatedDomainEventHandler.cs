using Gathering.Domain.Sessions;
using Gathering.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Gathering.Application.Sessions.DomainEvents;

/// <summary>
/// Handles the <see cref="SessionCreatedDomainEvent"/> raised when a new session is created.
/// </summary>
internal sealed class SessionCreatedDomainEventHandler(
    ILogger<SessionCreatedDomainEventHandler> logger) : IDomainEventHandler<SessionCreatedDomainEvent>
{
    public Task HandleAsync(SessionCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Session created: {SessionId}",
            domainEvent.SessionId);

        return Task.CompletedTask;
    }
}
