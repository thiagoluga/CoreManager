namespace Luga.Modules.Marketing.Shared.DTOs;

/// <summary>Payload accepted by <c>POST /api/marketing/contact</c>.</summary>
public sealed record ContactRequestDto(
    string Name,
    string Email,
    string? Company,
    string Message);
