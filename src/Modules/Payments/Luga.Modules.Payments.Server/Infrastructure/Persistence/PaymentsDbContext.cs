using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;
using Luga.Modules.Payments.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Payments.Server.Infrastructure.Persistence;

public sealed class PaymentsDbContext : LugaDbContextBase
{
    private readonly Guid _ambientTenantId;

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(tenantContext);
        _ambientTenantId = tenantContext.IsAuthenticated ? tenantContext.TenantId : Guid.Empty;
    }

    public override Guid CurrentTenantId => _ambientTenantId;

    public DbSet<TenantPlan> TenantPlans => Set<TenantPlan>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessedIntegrationEventConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
