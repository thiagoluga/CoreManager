namespace Luga.Modules.Customers.Shared.DTOs;

public sealed record UpdateCustomerRequest(
    string DisplayName,
    string Email,
    string? Phone,
    string? Document,
    string? Notes,
    bool IsActive,
    IReadOnlyDictionary<string, string>? CustomFields);
