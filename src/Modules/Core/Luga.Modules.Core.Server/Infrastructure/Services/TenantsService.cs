using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Contracts.DTOs;
using Luga.Modules.Core.Server.Application.Mappers;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;

namespace Luga.Modules.Core.Server.Infrastructure.Services;

/// <summary>
/// In-process <see cref="ITenantsService"/> implementation. Wraps
/// <see cref="ITenantRepository"/> with the Contract DTO shape. Lives in
/// <c>Infrastructure</c> because the Contract interface is the public surface.
/// </summary>
public sealed class TenantsService(ITenantRepository tenants) : ITenantsService
{
    private readonly ITenantRepository _tenants = tenants;

    /// <inheritdoc/>
    public async Task<TenantContractDto?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        Tenant? tenant = await _tenants.GetByIdAsync(tenantId, cancellationToken).ConfigureAwait(false);
        return tenant?.ToContractDto();
    }

    /// <inheritdoc/>
    public async Task<TenantContractDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        Tenant? tenant = await _tenants.GetBySlugAsync(slug, cancellationToken).ConfigureAwait(false);
        return tenant?.ToContractDto();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TenantContractDto>> GetByIdsAsync(
        IReadOnlyCollection<Guid> tenantIds,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Tenant> tenants = await _tenants.GetByIdsAsync(tenantIds, cancellationToken).ConfigureAwait(false);
        return [.. tenants.Select(t => t.ToContractDto())];
    }
}
