using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Domain.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.DbContextInterceptors;

/// <summary>
/// Converts <c>DELETE</c> into <c>UPDATE IsDeleted = true</c> for entities
/// implementing <see cref="ISoftDeletable"/>, capturing the audit fields
/// (DeletedBy, DeletedOn) from the ambient <see cref="ICurrentUser"/>.
/// </summary>
public sealed class SoftDeleteInterceptor(
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
            ApplySoftDelete(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplySoftDelete(DbContext context)
    {
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;
        Guid userId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty;
        string username = _currentUser.IsAuthenticated ? _currentUser.Username : "system";

        foreach (EntityEntry<ISoftDeletable> entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedById = userId;
            entry.Entity.DeletedByUsername = username;
            entry.Entity.DeletedOn = now;
        }
    }
}
