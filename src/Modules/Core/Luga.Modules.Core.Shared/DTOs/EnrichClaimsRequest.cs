namespace Luga.Modules.Core.Shared.DTOs;

/// <summary>
/// Payload sent by Entra External ID to the custom-claims provider endpoint
/// during the token issuance flow.
/// </summary>
/// <remarks>
/// Shape mirrors the Microsoft.Graph external authentication event schema
/// (only the fields Luga actually consumes are typed here; unknown fields are
/// ignored on deserialization).
/// </remarks>
public sealed record EnrichClaimsRequest(
    string Email,
    string? PreferredUsername);
