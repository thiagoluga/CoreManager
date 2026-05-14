using System.Security.Claims;

using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Server.Tenancy;

using Microsoft.AspNetCore.Http;

namespace Luga.BuildingBlocks.Server.Auth;

/// <summary>
/// <see cref="ICurrentUser"/> implementation backed by the request's
/// <see cref="HttpContext"/> claims.
/// </summary>
public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public bool IsAuthenticated => Principal()?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            ClaimsPrincipal? principal = Principal();
            string? id = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal?.FindFirst("sub")?.Value
                ?? principal?.FindFirst("oid")?.Value;
            return Guid.TryParse(id, out Guid parsed) ? parsed : Guid.Empty;
        }
    }

    public string Username
    {
        get
        {
            ClaimsPrincipal? principal = Principal();
            return principal?.Identity?.Name
                ?? principal?.FindFirst("preferred_username")?.Value
                ?? principal?.FindFirst(ClaimTypes.Email)?.Value
                ?? string.Empty;
        }
    }

    public string PreferredCulture
    {
        get
        {
            ClaimsPrincipal? principal = Principal();
            return principal is null ? string.Empty : TenantClaimsExtractor.GetPreferredCulture(principal);
        }
    }

    public IReadOnlySet<string> Permissions
    {
        get
        {
            ClaimsPrincipal? principal = Principal();
            if (principal is null)
            {
                return new HashSet<string>(StringComparer.Ordinal);
            }

            return TenantClaimsExtractor.GetPermissions(principal).ToHashSet(StringComparer.Ordinal);
        }
    }

    public bool HasPermission(string permission) =>
        !string.IsNullOrWhiteSpace(permission) && Permissions.Contains(permission);

    private ClaimsPrincipal? Principal() => _httpContextAccessor.HttpContext?.User;
}
