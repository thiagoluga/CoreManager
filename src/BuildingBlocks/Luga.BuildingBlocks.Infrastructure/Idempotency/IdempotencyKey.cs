namespace Luga.BuildingBlocks.Infrastructure.Idempotency;

/// <summary>
/// Row in <c>core.idempotency_keys</c>. Used to short-circuit retried mutating
/// requests that carry the same <c>Idempotency-Key</c> header (CLAUDE.md §16).
/// </summary>
public sealed class IdempotencyKey
{
    /// <summary>Client-supplied idempotency key (header value).</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>JSON-serialized response payload captured on the first execution.</summary>
    public string ResponsePayload { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the entry was created.</summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>UTC timestamp after which the entry can be purged. Defaults to created + 24h.</summary>
    public DateTime ExpiresOn { get; set; }
}
