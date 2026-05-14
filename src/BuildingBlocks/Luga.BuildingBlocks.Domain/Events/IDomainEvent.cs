namespace Luga.BuildingBlocks.Domain.Events;

/// <summary>
/// Evento interno do módulo, despachado em memória após <c>SaveChanges</c>.
/// </summary>
/// <remarks>
/// Diferente de <c>IIntegrationEvent</c> (público, versionado, atravessa fronteiras
/// de módulo via Outbox). Domain events são específicos do módulo e podem
/// evoluir livremente sem coordenação externa.
/// </remarks>
public interface IDomainEvent
{
    /// <summary>Identificador único do evento.</summary>
    Guid Id { get; }

    /// <summary>Quando o evento ocorreu (UTC).</summary>
    DateTime OccurredOn { get; }
}
