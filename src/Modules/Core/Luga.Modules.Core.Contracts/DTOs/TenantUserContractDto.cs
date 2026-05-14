namespace Luga.Modules.Core.Contracts.DTOs;

/// <summary>
/// Lightweight tenant-user snapshot exposed to other modules through
/// <see cref="IUsersService"/>.
/// </summary>
public sealed record TenantUserContractDto(
    Guid UserId,
    Guid TenantId,
    string Username,
    string DisplayName,
    string Role,
    string PreferredCulture,
    bool IsActive);
