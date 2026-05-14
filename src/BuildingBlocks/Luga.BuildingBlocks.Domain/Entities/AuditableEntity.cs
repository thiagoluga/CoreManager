using Luga.BuildingBlocks.Domain.Abstractions;

namespace Luga.BuildingBlocks.Domain.Entities;

/// <summary>
/// Entidade auditável: quem/quando criou e atualizou.
/// Campos populados pelo <c>AuditableEntityInterceptor</c>.
/// </summary>
public abstract class AuditableEntity : EntityBase, IAuditable
{
    /// <inheritdoc/>
    public Guid CreatedById { get; set; }

    /// <inheritdoc/>
    public string CreatedByUsername { get; set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime CreatedOn { get; set; }

    /// <inheritdoc/>
    public Guid? UpdatedById { get; set; }

    /// <inheritdoc/>
    public string? UpdatedByUsername { get; set; }

    /// <inheritdoc/>
    public DateTime? UpdatedOn { get; set; }
}
