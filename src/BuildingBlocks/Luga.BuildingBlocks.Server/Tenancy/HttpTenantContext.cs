using System.Security.Claims;

using Luga.BuildingBlocks.Application.Abstractions;

using Microsoft.AspNetCore.Http;

namespace Luga.BuildingBlocks.Server.Tenancy;

/// <summary>
/// <see cref="ITenantContext"/> backed by the current <see cref="HttpContext"/>
/// claims. Registered scoped — one instance per request.
/// </summary>
public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private const string DefaultMoneyCulture = "pt-BR";

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private Guid? _resolvedTenantId;

    public bool IsAuthenticated => GetResolvedTenantId() is not null;

    public Guid TenantId =>
        GetResolvedTenantId() ?? throw new InvalidOperationException(
            "No tenant resolved on the current request. Check authentication and the tenant_id claim.");

    public string TenantSlug
    {
        get
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;
            return principal is null ? string.Empty : TenantClaimsExtractor.GetTenantSlug(principal);
        }
    }

    public string DefaultCulture
    {
        get
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;
            string culture = principal is null ? string.Empty : TenantClaimsExtractor.GetDefaultCulture(principal);
            return string.IsNullOrWhiteSpace(culture) ? "pt-BR" : culture;
        }
    }

    public string MoneyCulture => DefaultMoneyCulture;

    private Guid? GetResolvedTenantId()
    {
        if (_resolvedTenantId is not null)
        {
            return _resolvedTenantId;
        }

        ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;
        _resolvedTenantId = principal is null ? null : TenantClaimsExtractor.GetTenantId(principal);
        return _resolvedTenantId;
    }
}
