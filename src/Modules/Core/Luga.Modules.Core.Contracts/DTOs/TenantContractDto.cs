namespace Luga.Modules.Core.Contracts.DTOs;

/// <summary>
/// Lightweight tenant snapshot exposed to other modules through
/// <see cref="ITenantsService"/>. Designed to be transport-friendly so the
/// shape stays stable when the Core module is extracted into its own service.
/// </summary>
public sealed record TenantContractDto(
    Guid TenantId,
    string Slug,
    string Name,
    string Status,
    string DefaultCulture);
