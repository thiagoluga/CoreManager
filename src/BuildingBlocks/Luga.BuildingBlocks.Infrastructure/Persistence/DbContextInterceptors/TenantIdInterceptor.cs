using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Domain.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.DbContextInterceptors;

/// <summary>
/// Auto-populates <see cref="IMultiTenant.TenantId"/> on entities being inserted,
/// pulling the value from <see cref="ITenantContext"/>. Update operations are not
/// touched (TenantId is immutable once an entity is created).
/// </summary>
public sealed class TenantIdInterceptor(ITenantContext tenantContext) : SaveChangesInterceptor
{
    private readonly ITenantContext _tenantContext = tenantContext;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context is not null && _tenantContext.IsAuthenticated)
        {
            Guid tenantId = _tenantContext.TenantId;
            foreach (EntityEntry<IMultiTenant> entry in eventData.Context.ChangeTracker.Entries<IMultiTenant>())
            {
                if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                {
                    entry.Entity.TenantId = tenantId;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
