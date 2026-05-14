using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;
using Luga.BuildingBlocks.IntegrationEvents;

using Microsoft.EntityFrameworkCore;

namespace Luga.BuildingBlocks.Infrastructure.Events;

/// <summary>
/// EF-backed <see cref="IProcessedEventStore"/>. Lives in the module's schema
/// (the table is mapped by the module's DbContext) so each module owns its
/// own idempotency log (CLAUDE.md §7.17).
/// </summary>
public sealed class ProcessedEventStore(LugaDbContextBase context, TimeProvider timeProvider) : IProcessedEventStore
{
    private readonly LugaDbContextBase _context = context;
    private readonly TimeProvider _timeProvider = timeProvider;

    public Task<bool> HasProcessedAsync(Guid eventId, string handlerName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);
        return _context.Set<ProcessedIntegrationEvent>()
            .AsNoTracking()
            .AnyAsync(p => p.EventId == eventId && p.HandlerName == handlerName, cancellationToken);
    }

    public Task MarkProcessedAsync(Guid eventId, string handlerName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);
        _context.Set<ProcessedIntegrationEvent>().Add(new ProcessedIntegrationEvent
        {
            EventId = eventId,
            HandlerName = handlerName,
            ProcessedOn = _timeProvider.GetUtcNow().UtcDateTime,
        });
        return Task.CompletedTask;
    }
}
