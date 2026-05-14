using System.Linq.Expressions;
using System.Reflection;

using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Domain.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Luga.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base class for every module's <c>DbContext</c>. Implements <see cref="IUnitOfWork"/>
/// and applies global query filters for multi-tenancy and soft delete plus
/// concurrency tokens for entities implementing <see cref="IConcurrencyAware"/>
/// (CLAUDE.md §7.6).
/// </summary>
/// <remarks>
/// Interceptors are registered once globally via
/// <c>PersistenceServiceCollectionExtensions.AddLugaPersistence</c> and added to
/// every module's <c>DbContextOptions</c>.
/// </remarks>
public abstract class LugaDbContextBase(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    private static readonly MethodInfo ApplyQueryFilterMethod = typeof(LugaDbContextBase)
        .GetMethod(nameof(ApplyGlobalQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// Ambient tenant id used by the multi-tenant query filter. Override in derived
    /// contexts that need to wire it from <c>ITenantContext</c>. Returning <see cref="Guid.Empty"/>
    /// effectively disables the filter (useful for migrations / seeds).
    /// </summary>
    public virtual Guid CurrentTenantId => Guid.Empty;

    /// <inheritdoc/>
    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureConcurrency(entityType);
            ConfigureGlobalQueryFilters(modelBuilder, entityType);
        }
    }

    private static void ConfigureConcurrency(IMutableEntityType entityType)
    {
        if (!typeof(IConcurrencyAware).IsAssignableFrom(entityType.ClrType))
        {
            return;
        }

        IMutableProperty? rowVersion = entityType.FindProperty(nameof(IConcurrencyAware.RowVersion));
        if (rowVersion is null)
        {
            return;
        }

        rowVersion.IsConcurrencyToken = true;
        rowVersion.ValueGenerated = ValueGenerated.OnAddOrUpdate;
        rowVersion.SetColumnType("rowversion");
    }

    private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder, IMutableEntityType entityType)
    {
        bool isTenant = typeof(IMultiTenant).IsAssignableFrom(entityType.ClrType);
        bool isSoftDeletable = typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType);

        if (!isTenant && !isSoftDeletable)
        {
            return;
        }

        MethodInfo generic = ApplyQueryFilterMethod.MakeGenericMethod(entityType.ClrType);
        generic.Invoke(this, [modelBuilder]);
    }

    private void ApplyGlobalQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        ParameterExpression entity = Expression.Parameter(typeof(TEntity), "e");

        Expression? predicate = null;

        if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
        {
            // e => EF.Property<Guid>(e, "TenantId") == this.CurrentTenantId
            //       || this.CurrentTenantId == Guid.Empty
            MethodInfo efProperty = typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(Guid));
            MethodCallExpression tenantProp = Expression.Call(
                efProperty,
                entity,
                Expression.Constant(nameof(IMultiTenant.TenantId)));
            MemberExpression current = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));
            BinaryExpression tenantMatches = Expression.Equal(tenantProp, current);
            BinaryExpression tenantBypass = Expression.Equal(current, Expression.Constant(Guid.Empty));
            predicate = Expression.OrElse(tenantMatches, tenantBypass);
        }

        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
        {
            // e => !EF.Property<bool>(e, "IsDeleted")
            MethodInfo efProperty = typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(bool));
            MethodCallExpression deletedProp = Expression.Call(
                efProperty,
                entity,
                Expression.Constant(nameof(ISoftDeletable.IsDeleted)));
            UnaryExpression notDeleted = Expression.Not(deletedProp);
            predicate = predicate is null ? notDeleted : Expression.AndAlso(predicate, notDeleted);
        }

        if (predicate is null)
        {
            return;
        }

        LambdaExpression lambda = Expression.Lambda(predicate, entity);
        modelBuilder.Entity<TEntity>().HasQueryFilter(lambda);
    }
}
