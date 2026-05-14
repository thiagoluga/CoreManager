namespace Luga.BuildingBlocks.Client.Auth;

/// <summary>
/// In-memory <see cref="IPermissionService"/>. Registered scoped so each user
/// session gets its own set.
/// </summary>
public sealed class PermissionService : IPermissionService
{
    private HashSet<string> _permissions = new(StringComparer.Ordinal);

    /// <inheritdoc/>
    public IReadOnlySet<string> Permissions => _permissions;

    /// <inheritdoc/>
    public bool HasPermission(string permission) =>
        !string.IsNullOrWhiteSpace(permission) && _permissions.Contains(permission);

    /// <inheritdoc/>
    public void SetPermissions(IEnumerable<string> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        _permissions = new HashSet<string>(permissions, StringComparer.Ordinal);
    }
}
