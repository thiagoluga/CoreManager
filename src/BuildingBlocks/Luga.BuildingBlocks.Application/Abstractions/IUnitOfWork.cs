namespace Luga.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Persists the unit-of-work boundary for a module's DbContext.
/// Handlers depend on this rather than on the concrete DbContext to keep
/// Application layer free of EF Core types.
/// </summary>
/// <remarks>
/// Each module's <c>LugaDbContextBase</c> implements this interface.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>Persists tracked changes and returns the number of affected rows.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
