namespace Luga.Modules.Customers.Shared.DTOs;

/// <summary>Lightweight projection used by list pages.</summary>
public sealed record CustomerSummaryDto(
    Guid Id,
    string DisplayName,
    string Email,
    string? Phone,
    bool IsActive,
    DateTime CreatedOn);
