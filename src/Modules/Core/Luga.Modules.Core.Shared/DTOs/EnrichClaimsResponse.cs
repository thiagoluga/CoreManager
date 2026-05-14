namespace Luga.Modules.Core.Shared.DTOs;

/// <summary>
/// Custom claims added to the JWT during the Entra External ID token flow.
/// Matches the names defined in <c>TenantClaimsExtractor</c>.
/// </summary>
public sealed record EnrichClaimsResponse(
    Guid? TenantId,
    string? TenantSlug,
    string? DefaultCulture,
    string? PreferredCulture,
    IReadOnlyList<string> Permissions);
