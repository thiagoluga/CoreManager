using Luga.BuildingBlocks.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Enums;

namespace Luga.Modules.Core.Server.Domain.Entities;

/// <summary>
/// Active subscription of a tenant to a <see cref="SubscriptionPlan"/>.
/// Multi-tenant (one row per tenant; the most recent <c>Active</c> row is the
/// effective subscription).
/// </summary>
public sealed class TenantSubscription : TenantEntity
{
    public Guid SubscriptionPlanId { get; set; }

    /// <summary>Cached display fields snapshotted at signup time.</summary>
    public string PlanCode { get; set; } = string.Empty;

    public string PlanName { get; set; } = string.Empty;

    public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    /// <summary>Modules the tenant has access to (cached from the plan).</summary>
    public IList<string> ActiveModules { get; set; } = [];
}
