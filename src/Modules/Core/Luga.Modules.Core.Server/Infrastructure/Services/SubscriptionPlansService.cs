using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Contracts.DTOs;

namespace Luga.Modules.Core.Server.Infrastructure.Services;

/// <summary>
/// MVP stub for <see cref="ISubscriptionPlansService"/>. Returns a hardcoded
/// public catalogue while §6.2 wires the real <c>SubscriptionPlan</c> entity,
/// repository and admin CRUD. Swap the stub once that lands.
/// </summary>
public sealed class SubscriptionPlansService : ISubscriptionPlansService
{
    private static readonly IReadOnlyList<PlanContractDto> Catalog =
    [
        new(
            Id: new Guid("00000000-0000-0000-0000-000000000001"),
            Code: "starter",
            Name: "Starter",
            Description: "Para começar: até 100 customers, fluxo manual de cobrança.",
            MonthlyPrice: 79m,
            AnnualPrice: 790m,
            BillingCycle: "Mensal",
            IncludedModules: ["core", "customers", "payments"],
            IsHighlighted: false,
            IsPublic: true),
        new(
            Id: new Guid("00000000-0000-0000-0000-000000000002"),
            Code: "pro",
            Name: "Pro",
            Description: "Crescimento: até 500 customers, integração com Asaas, notificações automáticas.",
            MonthlyPrice: 199m,
            AnnualPrice: 1990m,
            BillingCycle: "Mensal",
            IncludedModules: ["core", "customers", "payments", "personalization"],
            IsHighlighted: true,
            IsPublic: true),
        new(
            Id: new Guid("00000000-0000-0000-0000-000000000003"),
            Code: "business",
            Name: "Business",
            Description: "Operação madura: customers ilimitados, multi-usuário, automações avançadas.",
            MonthlyPrice: 399m,
            AnnualPrice: 3990m,
            BillingCycle: "Mensal",
            IncludedModules: ["core", "customers", "payments", "personalization"],
            IsHighlighted: false,
            IsPublic: true),
    ];

    public Task<IReadOnlyList<PlanContractDto>> GetPublicPlansAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Catalog);

    public Task<PlanContractDto?> GetByIdAsync(Guid planId, CancellationToken cancellationToken = default) =>
        Task.FromResult<PlanContractDto?>(Catalog.FirstOrDefault(p => p.Id == planId));

    public Task<IReadOnlyList<PlanContractDto>> GetByIdsAsync(IEnumerable<Guid> planIds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(planIds);
        HashSet<Guid> ids = [.. planIds];
        IReadOnlyList<PlanContractDto> result = [.. Catalog.Where(p => ids.Contains(p.Id))];
        return Task.FromResult(result);
    }
}
