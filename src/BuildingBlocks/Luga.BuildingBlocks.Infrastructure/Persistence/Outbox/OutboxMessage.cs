namespace Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;

/// <summary>
/// Row in <c>{schema}.outbox_messages</c>. Persisted in the same transaction
/// as the side effect that emits the integration event, then drained by
/// <c>OutboxProcessor</c> (Hangfire recurring job) — guaranteeing
/// at-least-once delivery even across crashes (CLAUDE.md §7.17).
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>Surrogate row id (different from the integration event id).</summary>
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>Integration event id (matches <c>IIntegrationEvent.Id</c>).</summary>
    public Guid EventId { get; set; }

    /// <summary>CLR full type name of the event for dispatcher reflection.</summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>JSON-serialized event payload.</summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>Tenant id captured at enqueue time (mirrors the originating entity).</summary>
    public Guid? TenantId { get; set; }

    /// <summary>UTC timestamp when the event was enqueued.</summary>
    public DateTime OccurredOn { get; set; }

    /// <summary>UTC timestamp when the message was dispatched. Null until processed.</summary>
    public DateTime? ProcessedOn { get; set; }

    /// <summary>Last error message captured during a failed attempt (null on success).</summary>
    public string? Error { get; set; }

    /// <summary>Retry count incremented on every failed dispatch attempt.</summary>
    public int RetryCount { get; set; }
}
