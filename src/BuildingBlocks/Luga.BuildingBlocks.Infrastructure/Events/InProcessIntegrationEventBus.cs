using Luga.BuildingBlocks.IntegrationEvents;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Infrastructure.Events;

/// <summary>
/// In-process implementation of <see cref="IIntegrationEventBus"/> for the monolith.
/// Resolves <see cref="IIntegrationEventHandler{TEvent}"/> instances from DI and
/// invokes them sequentially. After a module is extracted to its own service this
/// is swapped for a Service Bus implementation with zero handler-side changes
/// (CLAUDE.md §7.17).
/// </summary>
public sealed class InProcessIntegrationEventBus(
    IServiceScopeFactory scopeFactory,
    ILogger<InProcessIntegrationEventBus> logger) : IIntegrationEventBus
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<InProcessIntegrationEventBus> _logger = logger;

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

        IEnumerable<IIntegrationEventHandler<TEvent>> handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler<TEvent>>();

        foreach (IIntegrationEventHandler<TEvent> handler in handlers)
        {
            _logger.LogInformation(
                "Dispatching {EventType} (Id={EventId}) to {Handler}",
                typeof(TEvent).Name, integrationEvent.Id, handler.GetType().Name);

            await handler.HandleAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
