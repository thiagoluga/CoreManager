using Luga.BuildingBlocks.Domain.Abstractions;
using Luga.BuildingBlocks.Domain.Events;
using Luga.BuildingBlocks.IntegrationEvents;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Luga.BuildingBlocks.Infrastructure.Events;

/// <summary>
/// Collects pending <see cref="IDomainEvent"/> instances from tracked entities
/// after <c>SaveChanges</c> succeeds and dispatches them via MediatR. Integration
/// events (those also implementing <see cref="IIntegrationEvent"/>) are skipped
/// because <c>DomainEventToOutboxInterceptor</c> already enqueued them.
/// </summary>
/// <remarks>
/// Domain events fire only on success — failures roll back and leave nothing
/// to dispatch.
/// </remarks>
public sealed class DomainEventDispatcher(IMediator mediator)
{
    private readonly IMediator _mediator = mediator;

    /// <summary>Dispatches all pending domain events on entities tracked by <paramref name="context"/>.</summary>
    public async Task DispatchAsync(DbContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        EntityEntry<IHasDomainEvents>[] entries = [.. context.ChangeTracker.Entries<IHasDomainEvents>()];

        foreach (EntityEntry<IHasDomainEvents> entry in entries)
        {
            IDomainEvent[] events = [.. entry.Entity.DomainEvents.Where(e => e is not IIntegrationEvent)];
            entry.Entity.ClearDomainEvents();

            foreach (IDomainEvent domainEvent in events)
            {
                await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
