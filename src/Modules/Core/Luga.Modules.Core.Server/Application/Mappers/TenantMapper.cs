using Luga.Modules.Core.Contracts.DTOs;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Enums;
using Luga.Modules.Core.Shared.DTOs;

using Riok.Mapperly.Abstractions;

namespace Luga.Modules.Core.Server.Application.Mappers;

/// <summary>
/// Source-generated mappings between Core entities and Shared / Contracts DTOs
/// (CLAUDE.md §4.1 — Mapperly over AutoMapper). <c>RequiredMappingStrategy.Target</c>
/// keeps Mapperly from flagging audit/RowVersion source fields the DTOs intentionally drop.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TenantMapper
{
    /// <summary>Tenant → HTTP DTO.</summary>
    [MapProperty(nameof(Tenant.Id), nameof(TenantDto.TenantId))]
    public static partial TenantDto ToDto(this Tenant tenant);

    /// <summary>Tenant → cross-module contract DTO.</summary>
    [MapProperty(nameof(Tenant.Id), nameof(TenantContractDto.TenantId))]
    public static partial TenantContractDto ToContractDto(this Tenant tenant);

    /// <summary>TenantStatus enum → string used by DTOs.</summary>
    public static partial string MapStatus(TenantStatus status);
}
