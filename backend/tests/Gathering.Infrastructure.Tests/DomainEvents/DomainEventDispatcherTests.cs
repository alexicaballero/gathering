using FluentAssertions;
using Gathering.Application.Abstractions;
using Gathering.Infrastructure.DomainEvents;
using Gathering.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Gathering.Infrastructure.Tests.DomainEvents;

public class DomainEventDispatcherTests
{
  public sealed record TestDomainEvent(Guid Id) : IDomainEvent;

  [Fact]
  public async Task DispatchAsync_WhenHandlerRegistered_ShouldInvokeHandler()
  {
    // Arrange
    var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
    var services = new ServiceCollection();
    services.AddSingleton(handler);
    var serviceProvider = services.BuildServiceProvider();

    var dispatcher = new DomainEventDispatcher(
        serviceProvider,
        NullLogger<DomainEventDispatcher>.Instance);

    var domainEvent = new TestDomainEvent(Guid.NewGuid());

    // Act
    await dispatcher.DispatchAsync([domainEvent]);

    // Assert
    await handler.Received(1).HandleAsync(domainEvent, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task DispatchAsync_WhenNoHandlerRegistered_ShouldCompleteWithoutError()
  {
    // Arrange
    var services = new ServiceCollection();
    var serviceProvider = services.BuildServiceProvider();

    var dispatcher = new DomainEventDispatcher(
        serviceProvider,
        NullLogger<DomainEventDispatcher>.Instance);

    var domainEvent = new TestDomainEvent(Guid.NewGuid());

    // Act
    var act = async () => await dispatcher.DispatchAsync([domainEvent]);

    // Assert
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task DispatchAsync_WhenHandlerThrows_ShouldRethrowException()
  {
    // Arrange
    var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
    handler.HandleAsync(Arg.Any<TestDomainEvent>(), Arg.Any<CancellationToken>())
        .Returns(Task.FromException(new InvalidOperationException("Handler failed")));

    var services = new ServiceCollection();
    services.AddSingleton(handler);
    var serviceProvider = services.BuildServiceProvider();

    var dispatcher = new DomainEventDispatcher(
        serviceProvider,
        NullLogger<DomainEventDispatcher>.Instance);

    var domainEvent = new TestDomainEvent(Guid.NewGuid());

    // Act
    var act = async () => await dispatcher.DispatchAsync([domainEvent]);

    // Assert
    await act.Should().ThrowAsync<InvalidOperationException>();
  }

  [Fact]
  public async Task DispatchAsync_WithMultipleEvents_ShouldDispatchAll()
  {
    // Arrange
    var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
    var services = new ServiceCollection();
    services.AddSingleton(handler);
    var serviceProvider = services.BuildServiceProvider();

    var dispatcher = new DomainEventDispatcher(
        serviceProvider,
        NullLogger<DomainEventDispatcher>.Instance);

    var events = new List<IDomainEvent>
        {
            new TestDomainEvent(Guid.NewGuid()),
            new TestDomainEvent(Guid.NewGuid()),
            new TestDomainEvent(Guid.NewGuid())
        };

    // Act
    await dispatcher.DispatchAsync(events);

    // Assert
    await handler.Received(3).HandleAsync(Arg.Any<TestDomainEvent>(), Arg.Any<CancellationToken>());
  }
}
