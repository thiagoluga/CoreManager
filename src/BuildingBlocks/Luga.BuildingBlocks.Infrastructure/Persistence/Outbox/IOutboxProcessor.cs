namespace Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;

/// <summary>
/// Drains pending <see cref="OutboxMessage"/> rows for a single module's
/// <c>DbContext</c> and publishes each event via <see cref="IntegrationEvents.IIntegrationEventBus"/>.
/// Wired as a Hangfire recurring job (default cadence: every 10 seconds) per
/// module queue (CLAUDE.md §7.18).
/// </summary>
public interface IOutboxProcessor
{
    /// <summary>Processes up to <paramref name="batchSize"/> pending messages.</summary>
    Task ProcessAsync(int batchSize = 50, CancellationToken cancellationToken = default);
}
