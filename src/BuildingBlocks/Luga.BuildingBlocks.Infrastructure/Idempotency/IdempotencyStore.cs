using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.BuildingBlocks.Infrastructure.Idempotency;

/// <summary>
/// <see cref="IIdempotencyStore"/> backed by <c>core.idempotency_keys</c>.
/// The tracking context is expected to map <see cref="IdempotencyKey"/> — in
/// practice this is the Core module's DbContext.
/// </summary>
public sealed class IdempotencyStore(LugaDbContextBase context, TimeProvider timeProvider) : IIdempotencyStore
{
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromHours(24);

    private readonly LugaDbContextBase _context = context;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<string?> TryGetAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);

        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;
        IdempotencyKey? entry = await _context.Set<IdempotencyKey>()
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.Key == idempotencyKey && k.ExpiresOn > now, cancellationToken)
            .ConfigureAwait(false);
        return entry?.ResponsePayload;
    }

    public async Task SaveAsync(
        string idempotencyKey,
        string responsePayload,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);
        ArgumentNullException.ThrowIfNull(responsePayload);

        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;
        TimeSpan ttl = expiresIn ?? DefaultTtl;

        // Upsert: insert if missing, otherwise leave the original payload untouched.
        IdempotencyKey? existing = await _context.Set<IdempotencyKey>()
            .FirstOrDefaultAsync(k => k.Key == idempotencyKey, cancellationToken)
            .ConfigureAwait(false);

        if (existing is not null)
        {
            return;
        }

        _context.Set<IdempotencyKey>().Add(new IdempotencyKey
        {
            Key = idempotencyKey,
            ResponsePayload = responsePayload,
            CreatedOn = now,
            ExpiresOn = now.Add(ttl),
        });

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
