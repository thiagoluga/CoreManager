namespace Luga.Modules.Core.Shared.DTOs;

/// <summary>
/// Public signup payload — creates a new tenant plus the owning user in a
/// single call (CLAUDE.md §5.7 "Register tenant").
/// </summary>
public sealed record RegisterTenantRequest(
    string TenantName,
    string TenantSlug,
    string OwnerEmail,
    string OwnerDisplayName,
    string DefaultCulture = "pt-BR");
