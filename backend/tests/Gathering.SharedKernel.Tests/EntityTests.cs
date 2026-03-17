using FluentAssertions;
using Gathering.SharedKernel;

namespace Gathering.SharedKernel.Tests;

public class EntityTests
{
  // Concrete test double — Entity is abstract
  private sealed class TestEntity : Entity
  {
    public void RaiseEvent(IDomainEvent domainEvent) => Raise(domainEvent);
  }

  private sealed record TestDomainEvent(Guid Id) : IDomainEvent;

  [Fact]
  public void DomainEvents_WhenNoEventsRaised_ShouldBeEmpty()
  {
    var entity = new TestEntity();

    entity.DomainEvents.Should().BeEmpty();
  }

  [Fact]
  public void Raise_ShouldAddDomainEventToCollection()
  {
    var entity = new TestEntity();
    var domainEvent = new TestDomainEvent(Guid.NewGuid());

    entity.RaiseEvent(domainEvent);

    entity.DomainEvents.Should().ContainSingle()
        .Which.Should().Be(domainEvent);
  }

  [Fact]
  public void Raise_MultipleEvents_ShouldAddAllEventsInOrder()
  {
    var entity = new TestEntity();
    var event1 = new TestDomainEvent(Guid.NewGuid());
    var event2 = new TestDomainEvent(Guid.NewGuid());

    entity.RaiseEvent(event1);
    entity.RaiseEvent(event2);

    entity.DomainEvents.Should().HaveCount(2);
    entity.DomainEvents.First().Should().Be(event1);
    entity.DomainEvents.Last().Should().Be(event2);
  }

  [Fact]
  public void ClearDomainEvents_ShouldRemoveAllDomainEvents()
  {
    var entity = new TestEntity();
    entity.RaiseEvent(new TestDomainEvent(Guid.NewGuid()));
    entity.RaiseEvent(new TestDomainEvent(Guid.NewGuid()));

    entity.ClearDomainEvents();

    entity.DomainEvents.Should().BeEmpty();
  }

  [Fact]
  public void DomainEvents_ShouldReturnReadOnlyCollection()
  {
    var entity = new TestEntity();

    entity.DomainEvents.Should().BeAssignableTo<IReadOnlyCollection<IDomainEvent>>();
  }
}
