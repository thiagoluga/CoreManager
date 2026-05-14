namespace Luga.Modules.Core.Shared.DTOs;

/// <summary>
/// Tenant-user payload returned by <c>GET /api/users/me</c> and related endpoints.
/// </summary>
public sealed record TenantUserDto(
    Guid UserId,
    Guid TenantId,
    string Username,
    string DisplayName,
    string Role,
    string PreferredCulture,
    bool IsActive,
    IReadOnlyList<string> Permissions);
