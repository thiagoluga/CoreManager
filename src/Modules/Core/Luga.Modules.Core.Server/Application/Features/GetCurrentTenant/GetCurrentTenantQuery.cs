using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Core.Server.Application.Features.GetCurrentTenant;

/// <summary>
/// Returns the tenant the current authenticated user belongs to.
/// </summary>
public sealed record GetCurrentTenantQuery : IRequest<Result<TenantDto>>;
