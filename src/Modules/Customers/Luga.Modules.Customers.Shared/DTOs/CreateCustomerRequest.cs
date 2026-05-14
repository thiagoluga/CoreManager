namespace Luga.Modules.Customers.Shared.DTOs;

public sealed record CreateCustomerRequest(
    string DisplayName,
    string Email,
    string? Phone,
    string? Document,
    string? Notes,
    IReadOnlyDictionary<string, string>? CustomFields);
