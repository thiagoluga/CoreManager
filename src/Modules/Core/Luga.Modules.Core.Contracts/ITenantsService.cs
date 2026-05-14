using Luga.Modules.Core.Contracts.DTOs;

namespace Luga.Modules.Core.Contracts;

/// <summary>
/// In-process surface other modules use to query tenants. The implementation
/// lives in <c>Core.Server</c>; the same interface is re-fulfilled by a Refit
/// HTTP client when the Core module is extracted into its own service
/// (CLAUDE.md §3.2). Cross-module calls MUST use this interface — never reach
/// into the Core <c>DbContext</c> directly.
/// </summary>
public interface ITenantsService
{
    /// <summary>Fetches a tenant by id. Returns null when not found.</summary>
    Task<TenantContractDto?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Fetches a tenant by URL-safe slug. Returns null when not found.</summary>
    Task<TenantContractDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Batched lookup — fetches several tenants in one call. Defined since day 1
    /// to head off the N+1 footgun once Core becomes a microservice
    /// (CLAUDE.md §3.4 hazard 2).
    /// </summary>
    Task<IReadOnlyList<TenantContractDto>> GetByIdsAsync(
        IReadOnlyCollection<Guid> tenantIds,
        CancellationToken cancellationToken = default);
}
