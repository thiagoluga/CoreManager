namespace Luga.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Records idempotency keys for mutating requests so that a retried call with the
/// same <c>Idempotency-Key</c> header returns the cached response instead of
/// re-executing the handler. TTL is enforced at the storage layer.
/// </summary>
/// <remarks>
/// Concrete implementation lives in Infrastructure (table <c>core.idempotency_keys</c>).
/// Defined here so <see cref="Behaviors.IdempotencyBehavior{TRequest, TResponse}"/>
/// can depend on the abstraction.
/// </remarks>
public interface IIdempotencyStore
{
    /// <summary>
    /// Tries to fetch a previously recorded response for the given key.
    /// Returns the cached payload (JSON-serialized) when found, otherwise null.
    /// </summary>
    Task<string?> TryGetAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records the response payload (JSON) for the given key. Subsequent calls with the
    /// same key will short-circuit and return this payload.
    /// </summary>
    Task SaveAsync(
        string idempotencyKey,
        string responsePayload,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default);
}
