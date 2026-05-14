using Luga.BuildingBlocks.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Personalization.Server.Infrastructure.Persistence;

/// <summary>
/// Personalization's <see cref="LugaDbContextBase"/>. Schema starts empty
/// (the RBAC model is MVP-deferred to V1.1). Created now so the persistence
/// graph is in place and migrations can be added without restructuring the
/// host.
/// </summary>
public sealed class PersonalizationDbContext(DbContextOptions<PersonalizationDbContext> options)
    : LugaDbContextBase(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema("personalization");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonalizationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
