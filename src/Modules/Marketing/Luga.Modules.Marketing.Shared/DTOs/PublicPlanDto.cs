namespace Luga.Modules.Marketing.Shared.DTOs;

/// <summary>
/// Public-facing plan summary shown on the pricing page. The Marketing module
/// reads the catalog from Core via <c>ISubscriptionPlansService</c> and projects
/// only the fields a logged-out visitor should see (no internal sku codes,
/// discount metadata, etc.).
/// </summary>
public sealed record PublicPlanDto(
    string Code,
    string Name,
    string Description,
    decimal MonthlyPrice,
    decimal? AnnualPrice,
    string BillingCycle,
    IReadOnlyList<string> IncludedModules,
    bool IsHighlighted);
