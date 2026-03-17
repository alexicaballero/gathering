using Gathering.SharedKernel;

namespace Gathering.Application.Abstractions;

/// <summary>
/// Dispatches domain events to their registered handlers.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a collection of domain events to their corresponding handlers.
    /// Events are dispatched sequentially in the order provided.
    /// </summary>
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
