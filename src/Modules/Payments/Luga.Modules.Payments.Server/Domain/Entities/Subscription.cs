using Luga.BuildingBlocks.Domain.Entities;

namespace Luga.Modules.Payments.Server.Domain.Entities;

/// <summary>
/// Active subscription of a customer to one of the tenant's `TenantPlan`s.
/// </summary>
public sealed class Subscription : TenantEntity
{
    public Guid CustomerId { get; set; }

    public Guid TenantPlanId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public bool IsActive { get; set; } = true;
}
