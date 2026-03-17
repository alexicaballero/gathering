using Gathering.Application.Abstractions;
using Gathering.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gathering.Infrastructure.DomainEvents;

/// <summary>
/// Dispatches domain events to their registered <see cref="IDomainEventHandler{TDomainEvent}"/> handlers
/// by resolving them from the DI container at runtime.
/// </summary>
internal sealed class DomainEventDispatcher(
    IServiceProvider serviceProvider,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchEventAsync(domainEvent, cancellationToken);
        }
    }

    private async Task DispatchEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        var handlers = serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler is null)
            {
                continue;
            }

            try
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));

                if (method is not null)
                {
                    await (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error dispatching domain event {EventType}",
                    eventType.Name);

                throw;
            }
        }
    }
}
