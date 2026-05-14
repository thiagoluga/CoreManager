using Luga.BuildingBlocks.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Marketing.Server.Infrastructure.Persistence;

/// <summary>
/// Marketing's <see cref="LugaDbContextBase"/>. The schema starts empty —
/// the module mostly reads plan data from Core; any future Marketing-specific
/// entities (campaign metrics, lead capture log, …) live under the
/// <c>marketing</c> schema.
/// </summary>
public sealed class MarketingDbContext(DbContextOptions<MarketingDbContext> options) : LugaDbContextBase(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema("marketing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
