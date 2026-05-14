namespace Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;

/// <summary>
/// Row in <c>{schema}.processed_integration_events</c>. Compound key
/// (<see cref="EventId"/>, <see cref="HandlerName"/>) gives the at-most-once
/// guarantee per handler (CLAUDE.md §7.17).
/// </summary>
public sealed class ProcessedIntegrationEvent
{
    /// <summary>Integration event id.</summary>
    public Guid EventId { get; set; }

    /// <summary>Handler class name (e.g. <c>CustomerCreatedHandler</c>).</summary>
    public string HandlerName { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the handler completed.</summary>
    public DateTime ProcessedOn { get; set; }
}
