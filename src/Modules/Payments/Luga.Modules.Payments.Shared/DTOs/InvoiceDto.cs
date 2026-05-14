namespace Luga.Modules.Payments.Shared.DTOs;

public sealed record InvoiceDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    decimal Amount,
    DateTime DueDate,
    DateTime? PaidOn,
    string Status);
