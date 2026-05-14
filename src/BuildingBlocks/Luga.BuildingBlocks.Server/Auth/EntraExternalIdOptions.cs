namespace Luga.BuildingBlocks.Server.Auth;

/// <summary>
/// Strongly-typed bindings for the <c>EntraExternalId</c> section of <c>appsettings.json</c>.
/// </summary>
/// <remarks>
/// CLAUDE.md §7.13: single Entra External ID tenant for the whole product;
/// the <c>tenant_id</c> custom claim distinguishes app-tenants.
/// </remarks>
public sealed class EntraExternalIdOptions
{
    /// <summary>Configuration section name in <c>appsettings.json</c>.</summary>
    public const string SectionName = "EntraExternalId";

    /// <summary>Issuer URL (e.g. <c>https://luga.ciamlogin.com/{tenant}/v2.0</c>).</summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>API audience expected on incoming JWTs.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Public key endpoint metadata URL.</summary>
    public string MetadataAddress { get; set; } = string.Empty;

    /// <summary>Clock skew tolerance for token expiration validation.</summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(2);
}
