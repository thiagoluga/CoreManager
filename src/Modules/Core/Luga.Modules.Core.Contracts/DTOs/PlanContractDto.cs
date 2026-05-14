namespace Luga.Modules.Core.Contracts.DTOs;

/// <summary>
/// Cross-module projection of a subscription plan. Carries only what other
/// modules need to render or filter (no internal pricing strategy fields).
/// </summary>
public sealed record PlanContractDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    decimal MonthlyPrice,
    decimal? AnnualPrice,
    string BillingCycle,
    IReadOnlyList<string> IncludedModules,
    bool IsHighlighted,
    bool IsPublic);
