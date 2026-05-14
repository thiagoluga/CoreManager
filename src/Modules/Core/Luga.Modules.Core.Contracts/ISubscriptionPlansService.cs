using Luga.Modules.Core.Contracts.DTOs;

namespace Luga.Modules.Core.Contracts;

/// <summary>
/// Cross-module read surface for the SaaS subscription catalogue (CLAUDE.md §6.2).
/// Marketing displays public plans on the landing page; the Tenant signup flow
/// reads the same catalogue.
/// </summary>
public interface ISubscriptionPlansService
{
    /// <summary>Returns every plan flagged as public — for the marketing pricing page.</summary>
    Task<IReadOnlyList<PlanContractDto>> GetPublicPlansAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single plan by id (signup flow uses this).</summary>
    Task<PlanContractDto?> GetByIdAsync(Guid planId, CancellationToken cancellationToken = default);

    /// <summary>Batch lookup — CLAUDE.md §3.4 perigo 2 (no N+1 cross-service).</summary>
    Task<IReadOnlyList<PlanContractDto>> GetByIdsAsync(IEnumerable<Guid> planIds, CancellationToken cancellationToken = default);
}
