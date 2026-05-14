namespace Luga.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Marca a entidade como pertencente a um tenant.
/// <c>TenantId</c> é auto-populado em INSERT pelo <c>TenantIdInterceptor</c>
/// e queries são filtradas globalmente.
/// </summary>
/// <remarks>
/// Estratégia: shared database, shared schema, discriminator por TenantId.
/// </remarks>
public interface IMultiTenant
{
    /// <summary>Tenant ao qual a entidade pertence.</summary>
    Guid TenantId { get; set; }
}
