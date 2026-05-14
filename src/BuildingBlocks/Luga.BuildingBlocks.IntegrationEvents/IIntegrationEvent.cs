using Luga.BuildingBlocks.Domain.Events;

namespace Luga.BuildingBlocks.IntegrationEvents;

/// <summary>
/// Public event that crosses module boundaries. Always versioned (suffix
/// <c>V1</c>, <c>V2</c>...) and shipped through the outbox.
/// </summary>
/// <remarks>
/// Inherits <see cref="IDomainEvent"/> so it can be raised on an entity's
/// <c>DomainEvents</c> collection alongside internal events. The
/// <c>DomainEventToOutboxInterceptor</c> picks integration events out of that
/// collection at <c>SaveChanges</c> time and enqueues them; the
/// <c>DomainEventDispatcher</c> skips them so they are not re-dispatched
/// in-process (CLAUDE.md §3.1 / §3.4 hazard 3).
/// </remarks>
public interface IIntegrationEvent : IDomainEvent
{
    /// <summary>Contract version. Convention: matches the class-name suffix (V1, V2...).</summary>
    int Version { get; }
}
