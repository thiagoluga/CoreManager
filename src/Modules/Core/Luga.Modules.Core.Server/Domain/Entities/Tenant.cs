using Luga.BuildingBlocks.Domain.Entities;
using Luga.Modules.Core.Contracts.IntegrationEvents;
using Luga.Modules.Core.Server.Domain.Enums;
using Luga.Modules.Core.Server.Domain.Events;

namespace Luga.Modules.Core.Server.Domain.Entities;

/// <summary>
/// Tenant aggregate root. NOT <c>IMultiTenant</c> — it IS the tenant; rows live
/// in the global core schema, no <c>TenantId</c> column (CLAUDE.md §7.3).
/// </summary>
public sealed class Tenant : FullAuditableEntity
{
    private Tenant()
    {
        // EF Core
    }

    /// <summary>Tenant display name shown in the app bar and emails.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>URL-safe slug. Unique. Stable identifier in URLs.</summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>Lifecycle state.</summary>
    public TenantStatus Status { get; private set; } = TenantStatus.Active;

    /// <summary>Default UI culture for the tenant (e.g. <c>pt-BR</c>).</summary>
    public string DefaultCulture { get; private set; } = "pt-BR";

    /// <summary>
    /// Factory: builds a new tenant and a paired <see cref="TenantUser"/> for
    /// the owner. Raises both the domain event and the integration event so the
    /// outbox can ship the latter cross-module on commit.
    /// </summary>
    public static (Tenant Tenant, TenantUser Owner) Register(
        string name,
        string slug,
        string ownerEmail,
        string ownerDisplayName,
        string defaultCulture,
        TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerDisplayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultCulture);

        DateTime now = timeProvider.GetUtcNow().UtcDateTime;

        Tenant tenant = new()
        {
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            DefaultCulture = defaultCulture,
            Status = TenantStatus.Active,
        };

        TenantUser owner = TenantUser.CreateOwner(
            tenantId: tenant.Id,
            email: ownerEmail,
            displayName: ownerDisplayName,
            preferredCulture: defaultCulture);

        tenant.RaiseDomainEvent(new TenantRegisteredDomainEvent(
            Id: Guid.CreateVersion7(),
            OccurredOn: now,
            TenantId: tenant.Id,
            Slug: tenant.Slug));

        tenant.RaiseDomainEvent(new TenantCreatedIntegrationEventV1(
            Id: Guid.CreateVersion7(),
            OccurredOn: now,
            TenantId: tenant.Id,
            Slug: tenant.Slug,
            Name: tenant.Name,
            OwnerUserId: owner.Id,
            OwnerUsername: owner.Username,
            DefaultCulture: tenant.DefaultCulture));

        return (tenant, owner);
    }

    /// <summary>Suspends the tenant (typically called by the dunning workflow).</summary>
    public void Suspend()
    {
        if (Status == TenantStatus.Cancelled)
        {
            throw new InvalidOperationException("A cancelled tenant cannot be suspended.");
        }

        Status = TenantStatus.Suspended;
    }

    /// <summary>Reactivates a suspended tenant.</summary>
    public void Reactivate()
    {
        if (Status != TenantStatus.Suspended)
        {
            throw new InvalidOperationException("Only suspended tenants can be reactivated.");
        }

        Status = TenantStatus.Active;
    }

    /// <summary>Cancels the tenant. Subsequent soft delete is performed by the caller.</summary>
    public void Cancel()
    {
        Status = TenantStatus.Cancelled;
    }
}
