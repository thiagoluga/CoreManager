using Luga.BuildingBlocks.Domain.Common;

namespace Luga.Modules.Core.Server.Domain.Errors;

/// <summary>
/// Module-specific reusable errors. Module-shaped error codes
/// (<c>Core.{Reason}</c>) become RFC 7807 problem types at the HTTP edge.
/// </summary>
public static class CoreErrors
{
    /// <summary>Slug already used by another tenant.</summary>
    public static Error SlugTaken(string slug) =>
        new("Core.Tenant.Conflict", $"The slug '{slug}' is already in use.");

    /// <summary>Username already used inside the same tenant.</summary>
    public static Error UsernameTakenInTenant(string username) =>
        new("Core.TenantUser.Conflict", $"The username '{username}' is already registered for this tenant.");

    /// <summary>Tenant not found by id.</summary>
    public static Error TenantNotFound(Guid tenantId) =>
        new("Core.Tenant.NotFound", $"Tenant '{tenantId}' was not found.");

    /// <summary>Tenant user not found by id.</summary>
    public static Error UserNotFound(Guid userId) =>
        new("Core.TenantUser.NotFound", $"User '{userId}' was not found.");

    /// <summary>Operation requires the caller to be authenticated.</summary>
    public static Error NotAuthenticated() =>
        new("Core.Auth.Unauthorized", "Authentication required.");
}
