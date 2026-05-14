using Luga.BuildingBlocks.Domain.Entities;
using Luga.Modules.Payments.Server.Domain.Enums;

namespace Luga.Modules.Payments.Server.Domain.Entities;

/// <summary>
/// Invoice generated for a <c>Subscription</c>. Snapshots customer name and
/// amount at issuance time so changes to the customer profile don't rewrite
/// history.
/// </summary>
public sealed class Invoice : TenantEntity
{
    public Guid SubscriptionId { get; set; }

    public Guid CustomerId { get; set; }

    /// <summary>Customer name snapshot at issuance.</summary>
    public string CustomerName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? PaidOn { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    public string? Notes { get; set; }

    public void MarkAsPaid(DateTime paidOn)
    {
        if (Status != InvoiceStatus.Pending && Status != InvoiceStatus.Overdue)
        {
            throw new InvalidOperationException($"Cannot mark invoice in status {Status} as paid.");
        }

        Status = InvoiceStatus.Paid;
        PaidOn = paidOn;
    }
}
