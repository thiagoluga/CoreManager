namespace Luga.BuildingBlocks.IntegrationEvents;

/// <summary>
/// Evento público que atravessa fronteiras de módulo.
/// Sempre versionado (sufixo <c>V1</c>, <c>V2</c>...) e propagado via Outbox.
/// </summary>
/// <remarks>
/// CLAUDE.md §3.1 / §3.4 (perigo 3): contratos estáveis.
/// Breaking change exige nova versão coexistente (V2 ao lado de V1).
/// </remarks>
public interface IIntegrationEvent
{
    /// <summary>Identificador único do evento. Persistido no Outbox e usado em idempotência.</summary>
    Guid Id { get; }

    /// <summary>Quando o evento ocorreu (UTC).</summary>
    DateTime OccurredOn { get; }

    /// <summary>Versão do contrato. Convenção: igual ao sufixo do nome da classe (V1, V2...).</summary>
    int Version { get; }
}
