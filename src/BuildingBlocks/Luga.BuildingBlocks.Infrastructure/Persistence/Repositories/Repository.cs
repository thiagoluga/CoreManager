using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

using Luga.BuildingBlocks.Application.Pagination;
using Luga.BuildingBlocks.Application.Repositories;
using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Domain.Entities;
using Luga.BuildingBlocks.Domain.Errors;

using Microsoft.EntityFrameworkCore;

namespace Luga.BuildingBlocks.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic EF Core implementation of <see cref="IRepository{TEntity}"/>.
/// Module-specific repositories extend this and add domain-shaped queries
/// (CLAUDE.md §7.7).
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class Repository<TEntity>(LugaDbContextBase context) : IRepository<TEntity>
    where TEntity : EntityBase
{
    private static readonly string EntityName = typeof(TEntity).Name;

    /// <summary>Underlying DbContext, exposed for derived repositories.</summary>
    protected LugaDbContextBase Context { get; } = context;

    /// <summary>Tracked entity set for <typeparamref name="TEntity"/>.</summary>
    protected DbSet<TEntity> DbSet => Context.Set<TEntity>();

    /// <inheritdoc/>
    public virtual Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<Result<TEntity>> GetByIdRequiredAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return entity is not null
            ? entity
            : Result.Failure<TEntity>(GeneralErrors.NotFound(EntityName, id));
    }

    /// <inheritdoc/>
    public virtual Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(e => e.Id == id, cancellationToken);

    /// <inheritdoc/>
    public virtual void Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Add(entity);
    }

    /// <inheritdoc/>
    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        DbSet.AddRange(entities);
    }

    /// <inheritdoc/>
    public virtual void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Update(entity);
    }

    /// <inheritdoc/>
    public virtual void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Remove(entity);
    }

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> Query() => DbSet.AsQueryable();

    /// <inheritdoc/>
    public virtual Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return SpecificationEvaluator.Default
            .GetQuery(DbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<TResult?> FirstOrDefaultAsync<TResult>(
        ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return SpecificationEvaluator.Default
            .GetQuery(DbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<PagedList<TEntity>> ListAsync(
        ISpecification<TEntity>? specification,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        PagedRequest normalized = request.Normalized();

        IQueryable<TEntity> query = specification is null
            ? DbSet.AsQueryable()
            : SpecificationEvaluator.Default.GetQuery(DbSet.AsQueryable(), specification, evaluateCriteriaOnly: false);

        int total = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        List<TEntity> items = await query
            .Skip(normalized.Skip)
            .Take(normalized.Take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return PagedList<TEntity>.Create(items, total, normalized.Page, normalized.PageSize);
    }

    /// <inheritdoc/>
    public virtual async Task<PagedList<TResult>> ListAsync<TResult>(
        ISpecification<TEntity, TResult> specification,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        ArgumentNullException.ThrowIfNull(request);
        PagedRequest normalized = request.Normalized();

        IQueryable<TResult> query = SpecificationEvaluator.Default
            .GetQuery(DbSet.AsQueryable(), specification);

        int total = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        List<TResult> items = await query
            .Skip(normalized.Skip)
            .Take(normalized.Take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return PagedList<TResult>.Create(items, total, normalized.Page, normalized.PageSize);
    }
}
