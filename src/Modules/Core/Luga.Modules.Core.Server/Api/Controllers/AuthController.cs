using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Shared.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luga.Modules.Core.Server.Api.Controllers;

/// <summary>
/// Custom-claims provider endpoint invoked by Entra External ID during the
/// token issuance flow. Maps the authenticated user's email to the Luga tenant
/// scope and returns the extra claims the JWT must carry
/// (CLAUDE.md §5.7 / §7.13).
/// </summary>
/// <remarks>
/// Endpoint is anonymous because Entra calls it server-to-server with its own
/// API key; production wiring authenticates the caller via a shared secret
/// header validated in middleware (deferred — see §5.8).
/// </remarks>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(ITenantUserRepository users) : ControllerBase
{
    private readonly ITenantUserRepository _users = users;

    /// <summary>Returns the custom claims for a given user, or an empty payload when not registered.</summary>
    [HttpPost("enrich-claims")]
    public async Task<ActionResult<EnrichClaimsResponse>> EnrichClaimsAsync(
        [FromBody] EnrichClaimsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        TenantUser? user = await _users
            .GetByUsernameAnyTenantAsync(request.Email, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
        {
            return new EnrichClaimsResponse(null, null, null, null, []);
        }

        // Permissions wiring lives in Personalization (§6.4). For the MVP we return an empty list.
        return new EnrichClaimsResponse(
            TenantId: user.TenantId,
            TenantSlug: null,
            DefaultCulture: null,
            PreferredCulture: user.PreferredCulture,
            Permissions: []);
    }
}
