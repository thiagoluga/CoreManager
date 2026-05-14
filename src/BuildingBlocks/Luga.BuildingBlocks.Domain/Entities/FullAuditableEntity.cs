using Luga.BuildingBlocks.Domain.Abstractions;

namespace Luga.BuildingBlocks.Domain.Entities;

/// <summary>
/// Entidade auditável + soft-deletável. Adiciona campos de deleção lógica
/// ao <see cref="AuditableEntity"/>.
/// </summary>
public abstract class FullAuditableEntity : AuditableEntity, ISoftDeletable
{
    /// <inheritdoc/>
    public bool IsDeleted { get; set; }

    /// <inheritdoc/>
    public Guid? DeletedById { get; set; }

    /// <inheritdoc/>
    public string? DeletedByUsername { get; set; }

    /// <inheritdoc/>
    public DateTime? DeletedOn { get; set; }

    /// <inheritdoc/>
    public string? DeletionReason { get; set; }
}
