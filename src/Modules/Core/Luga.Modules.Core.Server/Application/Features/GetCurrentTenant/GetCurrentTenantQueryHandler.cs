using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Core.Server.Application.Mappers;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Errors;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Core.Server.Application.Features.GetCurrentTenant;

/// <summary>
/// Resolves the tenant id from <see cref="ITenantContext"/> and returns it,
/// short-circuiting with <c>Core.Auth.Unauthorized</c> when the request has no
/// tenant scope.
/// </summary>
public sealed class GetCurrentTenantQueryHandler(
    ITenantContext tenantContext,
    ITenantRepository tenants) : IRequestHandler<GetCurrentTenantQuery, Result<TenantDto>>
{
    private readonly ITenantContext _tenantContext = tenantContext;
    private readonly ITenantRepository _tenants = tenants;

    public async Task<Result<TenantDto>> Handle(GetCurrentTenantQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsAuthenticated)
        {
            return Result.Failure<TenantDto>(CoreErrors.NotAuthenticated());
        }

        Tenant? tenant = await _tenants.GetByIdAsync(_tenantContext.TenantId, cancellationToken).ConfigureAwait(false);
        return tenant is null
            ? Result.Failure<TenantDto>(CoreErrors.TenantNotFound(_tenantContext.TenantId))
            : Result.Success(tenant.ToDto());
    }
}
