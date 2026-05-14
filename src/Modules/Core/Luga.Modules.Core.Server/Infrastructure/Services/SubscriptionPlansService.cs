using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Contracts.DTOs;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Core.Server.Infrastructure.Services;

/// <summary>
/// EF-backed implementation of <see cref="ISubscriptionPlansService"/>. Seed
/// data is loaded by <c>CoreModuleInitializer</c>.
/// </summary>
public sealed class SubscriptionPlansService(CoreDbContext context) : ISubscriptionPlansService
{
    private readonly CoreDbContext _context = context;

    public async Task<IReadOnlyList<PlanContractDto>> GetPublicPlansAsync(CancellationToken cancellationToken = default)
    {
        List<SubscriptionPlan> plans = await _context.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsPublic)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return [.. plans.Select(Map)];
    }

    public async Task<PlanContractDto?> GetByIdAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        SubscriptionPlan? plan = await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken)
            .ConfigureAwait(false);

        return plan is null ? null : Map(plan);
    }

    public async Task<IReadOnlyList<PlanContractDto>> GetByIdsAsync(IEnumerable<Guid> planIds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(planIds);
        Guid[] ids = [.. planIds];

        List<SubscriptionPlan> plans = await _context.SubscriptionPlans
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return [.. plans.Select(Map)];
    }

    private static PlanContractDto Map(SubscriptionPlan p) => new(
        Id: p.Id,
        Code: p.Code,
        Name: p.Name,
        Description: p.Description,
        MonthlyPrice: p.MonthlyPrice,
        AnnualPrice: p.AnnualPrice,
        BillingCycle: p.DefaultBillingCycle.ToString(),
        IncludedModules: [.. p.IncludedModules],
        IsHighlighted: p.IsHighlighted,
        IsPublic: p.IsPublic);
}
