using Luga.BuildingBlocks.Domain.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.DbContextInterceptors;

/// <summary>
/// Records <see cref="IActivatable.ActivatedOn"/> / <see cref="IActivatable.DeactivatedOn"/>
/// transitions whenever <see cref="IActivatable.IsActive"/> changes value.
/// </summary>
public sealed class ActivationTrackingInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider = timeProvider;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context is not null)
        {
            ApplyActivationTracking(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyActivationTracking(DbContext context)
    {
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (EntityEntry<IActivatable> entry in context.ChangeTracker.Entries<IActivatable>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.IsActive && entry.Entity.ActivatedOn is null)
                {
                    entry.Entity.ActivatedOn = now;
                }

                continue;
            }

            if (entry.State != EntityState.Modified)
            {
                continue;
            }

            PropertyEntry<IActivatable, bool> property = entry.Property(e => e.IsActive);
            if (!property.IsModified)
            {
                continue;
            }

            if (entry.Entity.IsActive)
            {
                entry.Entity.ActivatedOn = now;
                entry.Entity.DeactivationReason = null;
            }
            else
            {
                entry.Entity.DeactivatedOn = now;
            }
        }
    }
}
