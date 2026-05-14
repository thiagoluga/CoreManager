namespace Luga.Modules.Core.Shared.DTOs;

/// <summary>
/// Full tenant payload returned by HTTP endpoints to authenticated users.
/// </summary>
public sealed record TenantDto(
    Guid TenantId,
    string Slug,
    string Name,
    string Status,
    string DefaultCulture,
    DateTime CreatedOn);
