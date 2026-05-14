using System.Text.Json;

using Luga.BuildingBlocks.IntegrationEvents;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;

/// <summary>
/// Generic outbox draining job. Each module registers a typed
/// <see cref="OutboxProcessor{TContext}"/> against its own DbContext.
/// </summary>
/// <typeparam name="TContext">Module DbContext (must map <see cref="OutboxMessage"/>).</typeparam>
public sealed class OutboxProcessor<TContext>(
    TContext context,
    IIntegrationEventBus bus,
    TimeProvider timeProvider,
    ILogger<OutboxProcessor<TContext>> logger) : IOutboxProcessor
    where TContext : DbContext
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly TContext _context = context;
    private readonly IIntegrationEventBus _bus = bus;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<OutboxProcessor<TContext>> _logger = logger;

    public async Task ProcessAsync(int batchSize = 50, CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive.");
        }

        DbSet<OutboxMessage> outbox = _context.Set<OutboxMessage>();

        List<OutboxMessage> pending = await outbox
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (OutboxMessage message in pending)
        {
            try
            {
                Type eventType = Type.GetType(message.EventType, throwOnError: false)
                    ?? throw new InvalidOperationException($"Unknown event type '{message.EventType}'.");

                object? deserialized = JsonSerializer.Deserialize(message.Payload, eventType, SerializerOptions);
                if (deserialized is not IIntegrationEvent integrationEvent)
                {
                    throw new InvalidOperationException(
                        $"Payload could not be deserialized to IIntegrationEvent (type='{message.EventType}').");
                }

                // The non-generic dispatch goes through reflection to invoke PublishAsync<TEvent>.
                Task task = (Task)typeof(IIntegrationEventBus)
                    .GetMethod(nameof(IIntegrationEventBus.PublishAsync))!
                    .MakeGenericMethod(eventType)
                    .Invoke(_bus, [integrationEvent, cancellationToken])!;
                await task.ConfigureAwait(false);

                message.ProcessedOn = _timeProvider.GetUtcNow().UtcDateTime;
                message.Error = null;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                message.RetryCount++;
                message.Error = ex.Message;
                _logger.LogError(
                    ex,
                    "Failed to dispatch outbox message {MessageId} (Type={EventType})",
                    message.Id, message.EventType);
            }
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
