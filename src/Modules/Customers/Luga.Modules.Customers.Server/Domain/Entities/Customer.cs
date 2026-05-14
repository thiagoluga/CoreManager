using Luga.BuildingBlocks.Domain.Entities;
using Luga.Modules.Customers.Contracts.IntegrationEvents;

namespace Luga.Modules.Customers.Server.Domain.Entities;

/// <summary>
/// Customer of a tenant (end customer in the B2B2C model). Multi-tenant: every
/// row carries <c>TenantId</c> and the query filter scopes reads to the
/// authenticated tenant.
/// </summary>
public sealed class Customer : TenantEntity
{
    public string DisplayName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    /// <summary>CPF or CNPJ (no dashes/dots) — opcional no MVP.</summary>
    public string? Document { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Custom-field values stored as a JSON column (CLAUDE.md §6.6 / ADR 030).</summary>
    public IDictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>Factory enforcing the invariants.</summary>
    public static Customer Create(
        string displayName,
        string email,
        string? phone,
        string? document,
        string? notes,
        IDictionary<string, string>? customFields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Customer customer = new()
        {
            DisplayName = displayName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Document = string.IsNullOrWhiteSpace(document) ? null : document.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CustomFields = customFields ?? new Dictionary<string, string>(StringComparer.Ordinal),
        };

        customer.RaiseDomainEvent(new CustomerCreatedIntegrationEventV1(
            CustomerId: customer.Id,
            TenantId: customer.TenantId,
            DisplayName: customer.DisplayName,
            Email: customer.Email,
            CreatedOn: DateTime.UtcNow));

        return customer;
    }
}
