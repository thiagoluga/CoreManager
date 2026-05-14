namespace Luga.BuildingBlocks.Client.Auth;

/// <summary>
/// Resolves authorization checks on the client. Backed by the permission
/// set returned for the user by the API, refreshed on login / role change.
/// </summary>
public interface IPermissionService
{
    /// <summary>All permission codes granted to the current user for the current tenant.</summary>
    IReadOnlySet<string> Permissions { get; }

    /// <summary>True when the user has the given permission code (empty/null → false).</summary>
    bool HasPermission(string permission);

    /// <summary>
    /// Replaces the in-memory permission set. Called by the host after login or
    /// when an API response updates the user's role assignments.
    /// </summary>
    void SetPermissions(IEnumerable<string> permissions);
}
