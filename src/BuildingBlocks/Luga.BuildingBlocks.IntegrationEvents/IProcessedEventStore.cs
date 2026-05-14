namespace Luga.BuildingBlocks.IntegrationEvents;

/// <summary>
/// Per-module store that records which integration events have already been
/// processed by which handler, providing the at-most-once-effective guarantee
/// on top of the outbox's at-least-once delivery (CLAUDE.md §7.17).
/// </summary>
public interface IProcessedEventStore
{
    /// <summary>True when this handler has already processed this event.</summary>
    Task<bool> HasProcessedAsync(Guid eventId, string handlerName, CancellationToken cancellationToken = default);

    /// <summary>Records that this handler has processed this event. Must run in the same transaction as the side effect.</summary>
    Task MarkProcessedAsync(Guid eventId, string handlerName, CancellationToken cancellationToken = default);
}
