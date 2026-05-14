namespace Luga.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Marca a entidade como sujeita a optimistic concurrency control.
/// Mapeada para <c>ROWVERSION</c> no SQL Server pelo EF Core.
/// </summary>
public interface IConcurrencyAware
{
    /// <summary>Token de concorrência. Atualizado a cada UPDATE pelo SQL Server.</summary>
    byte[] RowVersion { get; set; }
}
