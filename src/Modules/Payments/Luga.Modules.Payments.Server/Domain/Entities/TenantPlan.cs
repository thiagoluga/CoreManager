using Luga.BuildingBlocks.Domain.Entities;

namespace Luga.Modules.Payments.Server.Domain.Entities;

/// <summary>
/// Cobrança recorrente que o tenant oferece aos seus customers (por exemplo:
/// "Mensalidade musculação R$ 99/mês"). Diferente do <c>SubscriptionPlan</c>
/// (catálogo do Luga vendido aos tenants).
/// </summary>
public sealed class TenantPlan : TenantEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    /// <summary>Day of the month when the invoice should be generated (1-28).</summary>
    public int BillingDayOfMonth { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}
