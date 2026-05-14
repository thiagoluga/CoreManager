using System.Security.Claims;

namespace Luga.BuildingBlocks.Server.Tenancy;

/// <summary>
/// Helpers that pull tenant-related values out of a <see cref="ClaimsPrincipal"/>.
/// JWT issued by the Entra External ID custom claims provider carries
/// <c>tenant_id</c>, <c>tenant_slug</c>, <c>default_culture</c>, etc.
/// </summary>
public static class TenantClaimsExtractor
{
    /// <summary>Canonical claim name carrying the app-tenant id.</summary>
    public const string TenantIdClaim = "tenant_id";

    /// <summary>Canonical claim name carrying the tenant slug.</summary>
    public const string TenantSlugClaim = "tenant_slug";

    /// <summary>Canonical claim name carrying the tenant's default UI culture.</summary>
    public const string DefaultCultureClaim = "default_culture";

    /// <summary>Canonical claim name carrying the user's preferred culture.</summary>
    public const string PreferredCultureClaim = "preferred_culture";

    /// <summary>Canonical claim name carrying granted permission codes.</summary>
    public const string PermissionsClaim = "permissions";

    /// <summary>Returns the tenant id from the principal, or null when absent / malformed.</summary>
    public static Guid? GetTenantId(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        string? value = principal.FindFirst(TenantIdClaim)?.Value;
        return Guid.TryParse(value, out Guid id) ? id : null;
    }

    /// <summary>Returns the tenant slug, or empty when absent.</summary>
    public static string GetTenantSlug(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirst(TenantSlugClaim)?.Value ?? string.Empty;
    }

    /// <summary>Returns the tenant default culture, or empty when absent.</summary>
    public static string GetDefaultCulture(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirst(DefaultCultureClaim)?.Value ?? string.Empty;
    }

    /// <summary>Returns the user-preferred culture, or empty when absent.</summary>
    public static string GetPreferredCulture(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirst(PreferredCultureClaim)?.Value ?? string.Empty;
    }

    /// <summary>Returns granted permission codes from the principal.</summary>
    public static IEnumerable<string> GetPermissions(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindAll(PermissionsClaim).Select(c => c.Value);
    }
}
