using Luga.BuildingBlocks.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Enums;

namespace Luga.Modules.Core.Server.Domain.Entities;

/// <summary>
/// A subscription plan offered by Luga to its tenants. Owned by Luga (no
/// <see cref="Luga.BuildingBlocks.Domain.Abstractions.IMultiTenant"/>):
/// the catalog is product-wide, the same for every tenant.
/// </summary>
public sealed class SubscriptionPlan : FullAuditableEntity
{
    /// <summary>Stable short code used in URLs and config (e.g. <c>starter</c>).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Display name shown on the pricing page.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Marketing copy.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Monthly price in BRL.</summary>
    public decimal MonthlyPrice { get; set; }

    /// <summary>Annual price in BRL (null when no annual option is offered).</summary>
    public decimal? AnnualPrice { get; set; }

    /// <summary>Default billing cycle when the tenant subscribes.</summary>
    public BillingCycle DefaultBillingCycle { get; set; } = BillingCycle.Monthly;

    /// <summary>
    /// Module codes included in the plan, comma-separated (mapped as a value
    /// converter on EF). Order is significant for display in the UI.
    /// </summary>
    public IList<string> IncludedModules { get; set; } = [];

    /// <summary>True when this plan should be displayed on the public landing page.</summary>
    public bool IsPublic { get; set; }

    /// <summary>True when the plan should be visually emphasised (most popular badge).</summary>
    public bool IsHighlighted { get; set; }

    /// <summary>Sort order on the pricing page.</summary>
    public int DisplayOrder { get; set; }
}
