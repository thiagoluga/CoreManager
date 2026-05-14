using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Core.Server.Application.Features.RegisterTenant;

/// <summary>
/// Public signup command. Creates a new <c>Tenant</c> together with its owner
/// <c>TenantUser</c> atomically (single transaction).
/// </summary>
public sealed record RegisterTenantCommand(
    string TenantName,
    string TenantSlug,
    string OwnerEmail,
    string OwnerDisplayName,
    string DefaultCulture) : IRequest<Result<RegisterTenantResponse>>;
