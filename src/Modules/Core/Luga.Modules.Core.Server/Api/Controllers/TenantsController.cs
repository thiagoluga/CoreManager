using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Server.Http;
using Luga.Modules.Core.Server.Application.Features.GetCurrentTenant;
using Luga.Modules.Core.Server.Application.Features.RegisterTenant;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luga.Modules.Core.Server.Api.Controllers;

/// <summary>
/// Public-facing endpoints for tenants. Signup is anonymous; the rest requires
/// an authenticated tenant scope.
/// </summary>
[ApiController]
[Route("api/tenants")]
public sealed class TenantsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>Creates a new tenant + owner user.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterTenantRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<RegisterTenantResponse> result = await _mediator.Send(
            new RegisterTenantCommand(
                TenantName: request.TenantName,
                TenantSlug: request.TenantSlug,
                OwnerEmail: request.OwnerEmail,
                OwnerDisplayName: request.OwnerDisplayName,
                DefaultCulture: request.DefaultCulture),
            cancellationToken)
            .ConfigureAwait(false);

        return result.IsSuccess
            ? result.ToCreatedActionResult($"/api/tenants/{result.Value.Slug}")
            : result.ToActionResult();
    }

    /// <summary>Returns the tenant the current user belongs to.</summary>
    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentAsync(CancellationToken cancellationToken)
    {
        Result<TenantDto> result = await _mediator.Send(new GetCurrentTenantQuery(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult();
    }
}
