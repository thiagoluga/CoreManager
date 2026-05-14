using Luga.Modules.Core.Contracts.DTOs;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Enums;
using Luga.Modules.Core.Shared.DTOs;

using Riok.Mapperly.Abstractions;

namespace Luga.Modules.Core.Server.Application.Mappers;

/// <summary>
/// Source-generated mappings for <see cref="TenantUser"/>.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TenantUserMapper
{
    /// <summary>TenantUser → HTTP DTO (permissions injected by the caller from the RBAC store).</summary>
    public static TenantUserDto ToDto(this TenantUser user, IReadOnlyList<string> permissions)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(permissions);
        return new TenantUserDto(
            UserId: user.Id,
            TenantId: user.TenantId,
            Username: user.Username,
            DisplayName: user.DisplayName,
            Role: MapRole(user.Role),
            PreferredCulture: user.PreferredCulture,
            IsActive: user.IsActive,
            Permissions: permissions);
    }

    /// <summary>TenantUser → cross-module contract DTO.</summary>
    [MapProperty(nameof(TenantUser.Id), nameof(TenantUserContractDto.UserId))]
    public static partial TenantUserContractDto ToContractDto(this TenantUser user);

    /// <summary>Role enum → DTO string.</summary>
    public static partial string MapRole(TenantUserRole role);
}
