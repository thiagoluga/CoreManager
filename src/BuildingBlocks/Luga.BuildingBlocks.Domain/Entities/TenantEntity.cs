using Luga.BuildingBlocks.Domain.Abstractions;

namespace Luga.BuildingBlocks.Domain.Entities;

/// <summary>
/// Entidade que pertence a um tenant. Forma mais comum no Luga.
/// </summary>
/// <remarks>
/// <c>TenantId</c> é auto-populado em INSERT pelo <c>TenantIdInterceptor</c>
/// e queries são filtradas globalmente. Bypass exige <c>IgnoreQueryFilters()</c>
/// com justificativa em PR.
/// </remarks>
public abstract class TenantEntity : FullAuditableEntity, IMultiTenant
{
    /// <inheritdoc/>
    public Guid TenantId { get; set; }
}
