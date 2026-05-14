namespace Luga.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Ambient context exposing the tenant the current request belongs to.
/// Populated by <c>TenantContextMiddleware</c> from the <c>tenant_id</c> JWT claim.
/// </summary>
/// <remarks>
/// Scoped per request. Use this instead of reading the JWT directly.
/// Components that intentionally bypass tenancy (e.g. super-admin endpoints)
/// MUST do so through an explicit opt-out path, never by ignoring the context.
/// </remarks>
public interface ITenantContext
{
    /// <summary>True when a tenant has been resolved for the current scope.</summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Tenant id for the current request.
    /// Throws when accessed outside a tenant-scoped request — check <see cref="IsAuthenticated"/> first.
    /// </summary>
    Guid TenantId { get; }

    /// <summary>Stable, unique slug of the tenant (used in URLs, never displayed).</summary>
    string TenantSlug { get; }

    /// <summary>Default culture configured by the tenant (e.g. <c>pt-BR</c>).</summary>
    string DefaultCulture { get; }

    /// <summary>Culture used to format monetary values for the tenant. Brazilian tenants are always <c>pt-BR</c>.</summary>
    string MoneyCulture { get; }
}
