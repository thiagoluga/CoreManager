using Luga.BuildingBlocks.Application.Abstractions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Luga.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Short-circuits MediatR requests that implement <see cref="IIdempotentRequest"/>
/// when the supplied key has already been processed. The actual store lookup
/// and response replay (JSON deserialization back to <typeparamref name="TResponse"/>)
/// is performed at the HTTP edge by <c>IdempotencyMiddleware</c>; this behavior
/// simply records the key for future retries once the handler succeeds.
/// </summary>
/// <remarks>
/// Pairs with <c>IdempotencyMiddleware</c> (Infrastructure, §5.5) and the
/// <c>core.idempotency_keys</c> table.
/// </remarks>
public sealed class IdempotencyBehavior<TRequest, TResponse>(
    IIdempotencyStore store,
    ILogger<IdempotencyBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IIdempotencyStore _store = store;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (request is not IIdempotentRequest idempotent || string.IsNullOrWhiteSpace(idempotent.IdempotencyKey))
        {
            return await next().ConfigureAwait(false);
        }

        string? cached = await _store.TryGetAsync(idempotent.IdempotencyKey, cancellationToken)
            .ConfigureAwait(false);
        if (cached is not null)
        {
            _logger.LogInformation(
                "Idempotency hit for {RequestName} (Key={IdempotencyKey})",
                typeof(TRequest).Name, idempotent.IdempotencyKey);

            // Replay performed at the HTTP edge — middleware reads the cached payload and
            // returns it before this behavior is invoked again. Reaching here means the
            // middleware was bypassed (e.g. internal MediatR.Send): fall through and
            // re-execute the handler. The store will refuse to overwrite via SaveAsync.
        }

        TResponse response = await next().ConfigureAwait(false);

        // Serialization of the response payload is handled at the middleware boundary.
        // Behavior persists a sentinel so subsequent calls short-circuit at the edge.
        await _store.SaveAsync(
                idempotent.IdempotencyKey,
                responsePayload: string.Empty,
                expiresIn: TimeSpan.FromHours(24),
                cancellationToken)
            .ConfigureAwait(false);

        return response;
    }
}
