using Ardalis.Specification;

using Luga.BuildingBlocks.Application.Pagination;
using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Domain.Entities;

namespace Luga.BuildingBlocks.Application.Repositories;

/// <summary>
/// Generic repository contract for module entities (CLAUDE.md §7.7).
/// Implementations live in Infrastructure and wrap EF Core; queries respect
/// the global filters (tenant + soft-delete) automatically.
/// </summary>
/// <typeparam name="TEntity">Entity type. Must extend <see cref="EntityBase"/>.</typeparam>
/// <remarks>
/// Module-specific repositories extend this with domain-shaped methods
/// (e.g. <c>ICustomerRepository.GetByEmailAsync</c>). Cross-module access is
/// forbidden — use Contracts services instead.
/// </remarks>
public interface IRepository<TEntity>
    where TEntity : EntityBase
{
    /// <summary>Fetches an entity by id, or <c>null</c> when not found.</summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches an entity by id and wraps the result, returning a NotFound error
    /// when the entity is absent. Preferred form for handlers that propagate failures.
    /// </summary>
    Task<Result<TEntity>> GetByIdRequiredAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>True when an entity with the given id exists (respecting global filters).</summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Stages an insert. Persisted on <c>IUnitOfWork.SaveChangesAsync</c>.</summary>
    void Add(TEntity entity);

    /// <summary>Stages a bulk insert.</summary>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>Stages an update.</summary>
    void Update(TEntity entity);

    /// <summary>
    /// Stages a delete. For entities implementing <c>ISoftDeletable</c> this is
    /// converted to <c>IsDeleted = true</c> by the <c>SoftDeleteInterceptor</c>.
    /// </summary>
    void Remove(TEntity entity);

    /// <summary>
    /// Raw queryable for the entity. Use sparingly — prefer specifications.
    /// Global filters are applied; use <c>IgnoreQueryFilters()</c> explicitly
    /// when a bypass is intentional (and justify in PR).
    /// </summary>
    IQueryable<TEntity> Query();

    /// <summary>
    /// Returns a single matching entity for the given specification (or null).
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the projected single result for the given specification, or null.
    /// </summary>
    Task<TResult?> FirstOrDefaultAsync<TResult>(
        ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a materialized page of items matching the optional specification.
    /// </summary>
    Task<PagedList<TEntity>> ListAsync(
        ISpecification<TEntity>? specification,
        PagedRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a materialized page of projected results matching the specification.
    /// </summary>
    Task<PagedList<TResult>> ListAsync<TResult>(
        ISpecification<TEntity, TResult> specification,
        PagedRequest request,
        CancellationToken cancellationToken = default);
}
