namespace Luga.Modules.Customers.Contracts.DTOs;

/// <summary>
/// Cross-module projection of a customer. Carries only the fields other modules
/// need to render (Payments shows it on invoices, future Documents associates
/// files with it).
/// </summary>
public sealed record CustomerContractDto(
    Guid Id,
    string DisplayName,
    string Email,
    string? Phone,
    bool IsActive);
