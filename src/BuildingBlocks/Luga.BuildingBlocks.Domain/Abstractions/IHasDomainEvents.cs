using Luga.BuildingBlocks.Domain.Events;

namespace Luga.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Indica que a entidade emite domain events. O dispatcher coleta
/// <see cref="DomainEvents"/> após <c>SaveChanges</c> e os despacha
/// para handlers in-process; depois invoca <see cref="ClearDomainEvents"/>.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>Eventos pendentes acumulados nesta entidade.</summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>Registra um novo domain event para despacho posterior.</summary>
    void RaiseDomainEvent(IDomainEvent domainEvent);

    /// <summary>Limpa eventos após despacho.</summary>
    void ClearDomainEvents();
}
