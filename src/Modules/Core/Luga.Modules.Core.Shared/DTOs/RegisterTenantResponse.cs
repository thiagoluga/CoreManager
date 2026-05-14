namespace Luga.Modules.Core.Shared.DTOs;

/// <summary>
/// Result of a successful tenant signup.
/// </summary>
public sealed record RegisterTenantResponse(
    Guid TenantId,
    string Slug,
    Guid OwnerUserId);
