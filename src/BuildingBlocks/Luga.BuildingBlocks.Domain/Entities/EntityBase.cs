using Luga.BuildingBlocks.Domain.Abstractions;
using Luga.BuildingBlocks.Domain.Events;

namespace Luga.BuildingBlocks.Domain.Entities;

/// <summary>
/// Raiz da hierarquia de entidades. Garante Id (Guid v7 para ordenação temporal)
/// e <c>RowVersion</c> para optimistic concurrency.
/// Coleta de domain events fica aqui (toda entidade pode emitir).
/// </summary>
public abstract class EntityBase : IConcurrencyAware, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Identificador da entidade. Guid v7: prefixo temporal, melhor para índices.</summary>
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>Token de optimistic concurrency. Gerenciado pelo SQL Server (ROWVERSION).</summary>
    public byte[] RowVersion { get; set; } = [];

    /// <inheritdoc/>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <inheritdoc/>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <inheritdoc/>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
