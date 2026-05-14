using System.Text.Json;

using Luga.BuildingBlocks.Domain.Abstractions;
using Luga.BuildingBlocks.Domain.Events;
using Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;
using Luga.BuildingBlocks.IntegrationEvents;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.DbContextInterceptors;

/// <summary>
/// Captures <see cref="IIntegrationEvent"/> instances raised on entities and
/// writes them to <c>{schema}.outbox_messages</c> in the same transaction as
/// the originating side effect, providing at-least-once delivery (CLAUDE.md §7.17).
/// Domain-only events (those implementing <see cref="IDomainEvent"/> but not
/// <see cref="IIntegrationEvent"/>) are left for the in-process
/// <c>DomainEventDispatcher</c> to handle after <c>SaveChanges</c>.
/// </summary>
public sealed class DomainEventToOutboxInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context is not null)
        {
            EnqueueIntegrationEvents(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void EnqueueIntegrationEvents(DbContext context)
    {
        // Only contexts that map OutboxMessage can host the outbox; skip otherwise.
        if (context.Model.FindEntityType(typeof(OutboxMessage)) is null)
        {
            return;
        }

        DbSet<OutboxMessage> outbox = context.Set<OutboxMessage>();

        foreach (EntityEntry<IHasDomainEvents> entry in context.ChangeTracker.Entries<IHasDomainEvents>())
        {
            IReadOnlyCollection<IDomainEvent> events = entry.Entity.DomainEvents;
            if (events.Count == 0)
            {
                continue;
            }

            foreach (IDomainEvent domainEvent in events)
            {
                if (domainEvent is not IIntegrationEvent integrationEvent)
                {
                    continue;
                }

                Guid? tenantId = entry.Entity is IMultiTenant tenant ? tenant.TenantId : null;
                outbox.Add(new OutboxMessage
                {
                    EventId = integrationEvent.Id,
                    EventType = integrationEvent.GetType().AssemblyQualifiedName ?? integrationEvent.GetType().FullName!,
                    Payload = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType(), SerializerOptions),
                    TenantId = tenantId,
                    OccurredOn = integrationEvent.OccurredOn,
                });
            }

            // The in-process DomainEventDispatcher consumes (and clears) DomainEvents
            // AFTER SaveChanges. Do NOT clear here or domain events would be lost.
            // Note: integration events stay in the collection too — DomainEventDispatcher
            // ignores anything that is also an IIntegrationEvent.
        }
    }
}
