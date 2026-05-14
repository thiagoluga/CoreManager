using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Server.Http;
using Luga.Modules.Core.Server.Application.Features.GetMyProfile;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luga.Modules.Core.Server.Api.Controllers;

/// <summary>
/// Endpoints scoped to the authenticated user.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>Returns the current user's profile for the current tenant scope.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfileAsync(CancellationToken cancellationToken)
    {
        Result<TenantUserDto> result = await _mediator.Send(new GetMyProfileQuery(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult();
    }
}
