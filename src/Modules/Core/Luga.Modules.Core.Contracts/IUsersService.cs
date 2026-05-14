using Luga.Modules.Core.Contracts.DTOs;

namespace Luga.Modules.Core.Contracts;

/// <summary>
/// In-process surface other modules use to query tenant users.
/// </summary>
public interface IUsersService
{
    /// <summary>Fetches a tenant user by id. Returns null when not found.</summary>
    Task<TenantUserContractDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Fetches a tenant user by username scoped to a tenant. Returns null when not found.</summary>
    Task<TenantUserContractDto?> GetByUsernameAsync(
        Guid tenantId,
        string username,
        CancellationToken cancellationToken = default);

    /// <summary>Batched lookup — fetches several users in one call.</summary>
    Task<IReadOnlyList<TenantUserContractDto>> GetByIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default);
}
