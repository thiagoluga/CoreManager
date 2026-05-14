using Luga.BuildingBlocks.Server.Modules;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Enums;
using Luga.Modules.Core.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Core.Server;

/// <summary>
/// Seeds the platform-wide baseline (CLAUDE.md §7.11). Version 2 (the current
/// one) creates the public subscription plan catalog — Starter, Pro, Business.
/// Bumping <see cref="Version"/> re-runs <see cref="InitializeAsync"/>.
/// </summary>
public sealed class CoreModuleInitializer : IModuleInitializer
{
    /// <inheritdoc/>
    public string ModuleCode => "core";

    /// <inheritdoc/>
    public int Version => 2;

    /// <inheritdoc/>
    public async Task InitializeAsync(InitializationContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        CoreDbContext db = context.GetRequiredService<CoreDbContext>();

        await EnsurePlanAsync(db, BuildStarter(), cancellationToken).ConfigureAwait(false);
        await EnsurePlanAsync(db, BuildPro(), cancellationToken).ConfigureAwait(false);
        await EnsurePlanAsync(db, BuildBusiness(), cancellationToken).ConfigureAwait(false);

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task EnsurePlanAsync(CoreDbContext db, SubscriptionPlan plan, CancellationToken cancellationToken)
    {
        bool exists = await db.SubscriptionPlans
            .AnyAsync(p => p.Code == plan.Code, cancellationToken)
            .ConfigureAwait(false);

        if (!exists)
        {
            db.SubscriptionPlans.Add(plan);
        }
    }

    private static SubscriptionPlan BuildStarter() => new()
    {
        Code = "starter",
        Name = "Starter",
        Description = "Para começar: até 100 customers, fluxo manual de cobrança.",
        MonthlyPrice = 79m,
        AnnualPrice = 790m,
        DefaultBillingCycle = BillingCycle.Monthly,
        IncludedModules = ["core", "customers", "payments"],
        IsPublic = true,
        IsHighlighted = false,
        DisplayOrder = 10,
    };

    private static SubscriptionPlan BuildPro() => new()
    {
        Code = "pro",
        Name = "Pro",
        Description = "Crescimento: até 500 customers, integração com Asaas, notificações automáticas.",
        MonthlyPrice = 199m,
        AnnualPrice = 1990m,
        DefaultBillingCycle = BillingCycle.Monthly,
        IncludedModules = ["core", "customers", "payments", "personalization"],
        IsPublic = true,
        IsHighlighted = true,
        DisplayOrder = 20,
    };

    private static SubscriptionPlan BuildBusiness() => new()
    {
        Code = "business",
        Name = "Business",
        Description = "Operação madura: customers ilimitados, multi-usuário, automações avançadas.",
        MonthlyPrice = 399m,
        AnnualPrice = 3990m,
        DefaultBillingCycle = BillingCycle.Monthly,
        IncludedModules = ["core", "customers", "payments", "personalization"],
        IsPublic = true,
        IsHighlighted = false,
        DisplayOrder = 30,
    };
}
