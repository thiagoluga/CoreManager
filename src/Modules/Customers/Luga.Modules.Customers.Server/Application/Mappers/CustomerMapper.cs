using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Shared.DTOs;

namespace Luga.Modules.Customers.Server.Application.Mappers;

/// <summary>
/// Hand-rolled mapper kept small (Mapperly source-gen comes later when the
/// mapping surface grows beyond a couple of fields).
/// </summary>
internal static class CustomerMapper
{
    public static CustomerDto ToDto(Customer c) => new(
        Id: c.Id,
        DisplayName: c.DisplayName,
        Email: c.Email,
        Phone: c.Phone,
        Document: c.Document,
        Notes: c.Notes,
        IsActive: c.IsActive,
        CreatedOn: c.CreatedOn,
        UpdatedOn: c.UpdatedOn,
        CustomFields: new Dictionary<string, string>(c.CustomFields, StringComparer.Ordinal));

    public static CustomerSummaryDto ToSummary(Customer c) => new(
        Id: c.Id,
        DisplayName: c.DisplayName,
        Email: c.Email,
        Phone: c.Phone,
        IsActive: c.IsActive,
        CreatedOn: c.CreatedOn);
}
