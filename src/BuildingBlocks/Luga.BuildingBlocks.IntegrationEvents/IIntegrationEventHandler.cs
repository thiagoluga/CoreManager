namespace Luga.BuildingBlocks.IntegrationEvents;

/// <summary>
/// Implemented by modules that react to integration events published by other modules.
/// Implementations MUST be idempotent — the outbox guarantees at-least-once delivery
/// and may invoke the handler multiple times for the same event id.
/// </summary>
/// <typeparam name="TEvent">Integration event payload type.</typeparam>
/// <remarks>
/// Idempotency check pattern (CLAUDE.md §7.17):
/// <code>
/// if (await _processedEvents.HasProcessedAsync(evt.Id, nameof(MyHandler), ct)) return;
/// // ... do work ...
/// await _processedEvents.MarkProcessedAsync(evt.Id, nameof(MyHandler), ct);
/// await _uow.SaveChangesAsync(ct);
/// </code>
/// </remarks>
public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IIntegrationEvent
{
    /// <summary>Processes the integration event.</summary>
    Task HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken = default);
}
