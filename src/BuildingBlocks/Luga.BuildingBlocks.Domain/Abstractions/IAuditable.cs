namespace Luga.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Marca a entidade como auditável: armazena quem criou/atualizou e quando.
/// Populado automaticamente por <c>AuditableEntityInterceptor</c>.
/// </summary>
/// <remarks>
/// Guardamos tanto <c>UserId</c> quanto <c>Username</c> (snapshot) para que
/// renomes ou exclusões de user não corrompam o histórico.
/// </remarks>
public interface IAuditable
{
    /// <summary>Id do user que criou a entidade.</summary>
    Guid CreatedById { get; set; }

    /// <summary>Snapshot do username no momento da criação.</summary>
    string CreatedByUsername { get; set; }

    /// <summary>Timestamp UTC da criação.</summary>
    DateTime CreatedOn { get; set; }

    /// <summary>Id do user que fez a última atualização.</summary>
    Guid? UpdatedById { get; set; }

    /// <summary>Snapshot do username no momento da atualização.</summary>
    string? UpdatedByUsername { get; set; }

    /// <summary>Timestamp UTC da última atualização.</summary>
    DateTime? UpdatedOn { get; set; }
}
