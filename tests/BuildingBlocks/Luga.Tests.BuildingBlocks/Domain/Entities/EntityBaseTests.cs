using Luga.BuildingBlocks.Domain.Entities;
using Luga.BuildingBlocks.Domain.Events;

namespace Luga.Tests.BuildingBlocks.Domain.Entities;

public sealed class EntityBaseTests
{
    [Fact]
    public void Constructor_AssignsNonEmptyGuid()
    {
        TestEntity entity = new();

        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_AssignsGuidVersion7()
    {
        TestEntity entity = new();

        entity.Id.Version.Should().Be(7);
    }

    [Fact]
    public void RaiseDomainEvent_AccumulatesEvents()
    {
        TestEntity entity = new();
        TestDomainEvent first = new();
        TestDomainEvent second = new();

        entity.RaiseDomainEvent(first);
        entity.RaiseDomainEvent(second);

        entity.DomainEvents.Should().HaveCount(2);
        entity.DomainEvents.Should().ContainInOrder(first, second);
    }

    [Fact]
    public void RaiseDomainEvent_WithNull_Throws()
    {
        TestEntity entity = new();

        Action act = () => entity.RaiseDomainEvent(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ClearDomainEvents_EmptiesCollection()
    {
        TestEntity entity = new();
        entity.RaiseDomainEvent(new TestDomainEvent());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    private sealed class TestEntity : EntityBase;

    private sealed record TestDomainEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.CreateVersion7();

        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
