using Luga.BuildingBlocks.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;

namespace Luga.Modules.Core.Server.Application.Repositories;

/// <summary>
/// Tenant-user repository.
/// </summary>
public interface ITenantUserRepository : IRepository<TenantUser>
{
    /// <summary>Fetches a tenant user by username scoped to the current tenant.</summary>
    Task<TenantUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>Batched lookup by id.</summary>
    Task<IReadOnlyList<TenantUser>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default);

    /// <summary>Fetches a tenant user by username across all tenants (used by the claims provider).</summary>
    Task<TenantUser?> GetByUsernameAnyTenantAsync(string username, CancellationToken cancellationToken = default);
}
