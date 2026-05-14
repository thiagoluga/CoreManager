using Luga.BuildingBlocks.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;

namespace Luga.Modules.Core.Server.Application.Repositories;

/// <summary>
/// Tenant repository — extends the generic <see cref="IRepository{TEntity}"/>
/// with domain-shaped lookups.
/// </summary>
public interface ITenantRepository : IRepository<Tenant>
{
    /// <summary>True if a tenant already uses the given slug.</summary>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Fetches a tenant by slug, or null when missing.</summary>
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Batched lookup by id (always defined to head off the cross-service N+1 footgun).</summary>
    Task<IReadOnlyList<Tenant>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default);
}
