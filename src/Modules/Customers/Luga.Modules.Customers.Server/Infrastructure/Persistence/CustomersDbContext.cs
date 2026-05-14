using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;
using Luga.Modules.Customers.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Customers.Server.Infrastructure.Persistence;

public sealed class CustomersDbContext : LugaDbContextBase
{
    private readonly Guid _ambientTenantId;

    public CustomersDbContext(DbContextOptions<CustomersDbContext> options)
        : base(options)
    {
    }

    public CustomersDbContext(DbContextOptions<CustomersDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(tenantContext);
        _ambientTenantId = tenantContext.IsAuthenticated ? tenantContext.TenantId : Guid.Empty;
    }

    public override Guid CurrentTenantId => _ambientTenantId;

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema("customers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessedIntegrationEventConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
