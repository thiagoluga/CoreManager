namespace Luga.Modules.Core.Server.Domain.Enums;

/// <summary>
/// Baseline role of a user within a tenant. The Personalization module adds
/// fine-grained role/permission management on top of this baseline.
/// </summary>
public enum TenantUserRole
{
    /// <summary>Created the tenant. Cannot be removed.</summary>
    Owner = 0,

    /// <summary>Full administrative privileges except tenant ownership transfer.</summary>
    Admin = 1,

    /// <summary>Day-to-day management within their scope.</summary>
    Manager = 2,

    /// <summary>Operational access (e.g. registering payments).</summary>
    Operator = 3,

    /// <summary>Read-only access.</summary>
    Viewer = 4,
}
