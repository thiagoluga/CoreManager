using Luga.BuildingBlocks.Domain.Abstractions;
using Luga.BuildingBlocks.Domain.Entities;

namespace Luga.Tests.BuildingBlocks.Domain.Entities;

/// <summary>
/// Garante que a hierarquia implementa os marker interfaces corretos.
/// </summary>
public sealed class EntityHierarchyTests
{
    [Fact]
    public void EntityBase_Implements_IConcurrencyAware_And_IHasDomainEvents()
    {
        typeof(EntityBase).Should().BeAssignableTo<IConcurrencyAware>();
        typeof(EntityBase).Should().BeAssignableTo<IHasDomainEvents>();
    }

    [Fact]
    public void AuditableEntity_AddsIAuditable()
    {
        typeof(AuditableEntity).Should().BeAssignableTo<EntityBase>();
        typeof(AuditableEntity).Should().BeAssignableTo<IAuditable>();
    }

    [Fact]
    public void FullAuditableEntity_AddsISoftDeletable()
    {
        typeof(FullAuditableEntity).Should().BeAssignableTo<AuditableEntity>();
        typeof(FullAuditableEntity).Should().BeAssignableTo<ISoftDeletable>();
    }

    [Fact]
    public void TenantEntity_IsFullAuditableAndIMultiTenant()
    {
        typeof(TenantEntity).Should().BeAssignableTo<FullAuditableEntity>();
        typeof(TenantEntity).Should().BeAssignableTo<IMultiTenant>();
        typeof(TenantEntity).Should().BeAssignableTo<IAuditable>();
        typeof(TenantEntity).Should().BeAssignableTo<ISoftDeletable>();
        typeof(TenantEntity).Should().BeAssignableTo<IConcurrencyAware>();
        typeof(TenantEntity).Should().BeAssignableTo<IHasDomainEvents>();
    }
}
