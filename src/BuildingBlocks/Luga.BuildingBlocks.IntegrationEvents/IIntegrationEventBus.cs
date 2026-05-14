namespace Luga.BuildingBlocks.IntegrationEvents;

/// <summary>
/// Dispatches integration events to registered handlers.
/// </summary>
/// <remarks>
/// In the monolith this is fulfilled by an in-process bus that reads from the
/// outbox and calls handlers via DI. After a module is extracted to its own service
/// the same interface is fulfilled by an Azure Service Bus implementation — call sites
/// do not change (CLAUDE.md §3.2).
/// </remarks>
public interface IIntegrationEventBus
{
    /// <summary>Publishes an integration event to all registered handlers.</summary>
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
