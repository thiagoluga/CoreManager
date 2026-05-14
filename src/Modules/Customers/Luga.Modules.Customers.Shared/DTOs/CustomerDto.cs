namespace Luga.Modules.Customers.Shared.DTOs;

/// <summary>Detailed customer projection returned by the API for edit / detail pages.</summary>
public sealed record CustomerDto(
    Guid Id,
    string DisplayName,
    string Email,
    string? Phone,
    string? Document,
    string? Notes,
    bool IsActive,
    DateTime CreatedOn,
    DateTime? UpdatedOn,
    IReadOnlyDictionary<string, string> CustomFields);
