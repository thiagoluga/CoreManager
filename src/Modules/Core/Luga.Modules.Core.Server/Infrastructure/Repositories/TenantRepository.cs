using Luga.BuildingBlocks.Infrastructure.Persistence.Repositories;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Core.Server.Infrastructure.Repositories;

public sealed class TenantRepository(CoreDbContext context) : Repository<Tenant>(context), ITenantRepository
{
    private readonly CoreDbContext _context = context;

    /// <inheritdoc/>
    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        string normalized = slug.ToLowerInvariant();
        return _context.Tenants.AnyAsync(t => t.Slug == normalized, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        string normalized = slug.ToLowerInvariant();
        return _context.Tenants.FirstOrDefaultAsync(t => t.Slug == normalized, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Tenant>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (ids.Count == 0)
        {
            return [];
        }

        return await _context.Tenants
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
