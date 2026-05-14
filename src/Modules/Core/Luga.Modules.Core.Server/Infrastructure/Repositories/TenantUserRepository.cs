using Luga.BuildingBlocks.Infrastructure.Persistence.Repositories;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Core.Server.Infrastructure.Repositories;

public sealed class TenantUserRepository(CoreDbContext context) : Repository<TenantUser>(context), ITenantUserRepository
{
    private readonly CoreDbContext _context = context;

    /// <inheritdoc/>
    public Task<TenantUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        string normalized = username.ToLowerInvariant();
        return _context.TenantUsers.FirstOrDefaultAsync(u => u.Username == normalized, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TenantUser>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (ids.Count == 0)
        {
            return [];
        }

        return await _context.TenantUsers
            .Where(u => ids.Contains(u.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<TenantUser?> GetByUsernameAnyTenantAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        string normalized = username.ToLowerInvariant();

        // Bypass tenant filter: claims enrichment looks up the user before a tenant scope exists.
        return _context.TenantUsers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Username == normalized, cancellationToken);
    }
}
