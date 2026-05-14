using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Contracts.DTOs;
using Luga.Modules.Marketing.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Marketing.Server.Application;

/// <summary>
/// Reads the public plan catalogue from Core and projects it into the
/// front-end DTO shape.
/// </summary>
public sealed class GetPublicPlansHandler(ISubscriptionPlansService plans)
    : IRequestHandler<GetPublicPlansQuery, Result<IReadOnlyList<PublicPlanDto>>>
{
    private readonly ISubscriptionPlansService _plans = plans;

    public async Task<Result<IReadOnlyList<PublicPlanDto>>> Handle(GetPublicPlansQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<PlanContractDto> source = await _plans.GetPublicPlansAsync(cancellationToken).ConfigureAwait(false);

        IReadOnlyList<PublicPlanDto> projection =
        [
            .. source.Select(p => new PublicPlanDto(
                Code: p.Code,
                Name: p.Name,
                Description: p.Description,
                MonthlyPrice: p.MonthlyPrice,
                AnnualPrice: p.AnnualPrice,
                BillingCycle: p.BillingCycle,
                IncludedModules: p.IncludedModules,
                IsHighlighted: p.IsHighlighted)),
        ];

        return Result.Success(projection);
    }
}
