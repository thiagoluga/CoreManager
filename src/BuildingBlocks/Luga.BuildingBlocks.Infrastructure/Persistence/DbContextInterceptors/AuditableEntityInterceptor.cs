using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Domain.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.DbContextInterceptors;

/// <summary>
/// Populates <see cref="IAuditable"/> fields (CreatedBy/UpdatedBy and timestamps)
/// from the ambient <see cref="ICurrentUser"/> and <see cref="TimeProvider"/>.
/// </summary>
public sealed class AuditableEntityInterceptor(
    ICurrentUser currentUser,
    TimeProvider timeProvider) : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _timeProvider = timeProvider;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context is not null)
        {
            ApplyAuditFields(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditFields(DbContext context)
    {
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;
        Guid userId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty;
        string username = _currentUser.IsAuthenticated ? _currentUser.Username : "system";

        foreach (EntityEntry<IAuditable> entry in context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedById = userId;
                    entry.Entity.CreatedByUsername = username;
                    entry.Entity.CreatedOn = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedById = userId;
                    entry.Entity.UpdatedByUsername = username;
                    entry.Entity.UpdatedOn = now;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }
        }
    }
}
