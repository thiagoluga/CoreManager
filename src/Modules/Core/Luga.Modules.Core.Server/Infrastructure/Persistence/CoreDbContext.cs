using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Infrastructure.Idempotency;
using Luga.BuildingBlocks.Infrastructure.Persistence;
using Luga.BuildingBlocks.Infrastructure.Persistence.Migrations;
using Luga.BuildingBlocks.Infrastructure.Persistence.Outbox;
using Luga.Modules.Core.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Core.Server.Infrastructure.Persistence;

/// <summary>
/// EF Core context for the Core module. Owns the <c>core</c> schema plus the
/// shared <c>core.idempotency_keys</c> and <c>core.module_initializations</c>
/// tables (the only tables that intentionally live in <c>core</c> by design —
/// every other module hosts these tables in its own schema).
/// </summary>
public sealed class CoreDbContext : LugaDbContextBase
{
    private readonly Guid _ambientTenantId;

    /// <summary>Design-time / pooled constructor used by EF tooling.</summary>
    public CoreDbContext(DbContextOptions<CoreDbContext> options)
        : base(options)
    {
    }

    /// <summary>DI constructor that captures the current tenant scope.</summary>
    public CoreDbContext(DbContextOptions<CoreDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(tenantContext);
        _ambientTenantId = tenantContext.IsAuthenticated ? tenantContext.TenantId : Guid.Empty;
    }

    /// <inheritdoc/>
    public override Guid CurrentTenantId => _ambientTenantId;

    /// <summary>Tenant aggregate root.</summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();

    /// <summary>Tenant user.</summary>
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();

    /// <summary>Outbox messages for Core.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>Processed integration events log (Core handlers idempotency).</summary>
    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();

    /// <summary>Idempotency keys for HTTP retries.</summary>
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();

    /// <summary>Module initialization tracking (lives in <c>core</c> for the whole platform).</summary>
    public DbSet<ModuleInitialization> ModuleInitializations => Set<ModuleInitialization>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema("core");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessedIntegrationEventConfiguration());
        modelBuilder.ApplyConfiguration(new IdempotencyKeyConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleInitializationConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
