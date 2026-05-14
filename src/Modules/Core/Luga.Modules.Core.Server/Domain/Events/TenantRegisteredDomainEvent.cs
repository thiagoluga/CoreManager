using Luga.BuildingBlocks.Domain.Events;

namespace Luga.Modules.Core.Server.Domain.Events;

/// <summary>
/// In-process notification fired when a new tenant finishes registration.
/// Used by the Core module itself for follow-up logic (e.g. allocating a
/// default subscription). Cross-module reactions use
/// <c>TenantCreatedIntegrationEventV1</c> instead (CLAUDE.md §7.17).
/// </summary>
public sealed record TenantRegisteredDomainEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid TenantId,
    string Slug) : IDomainEvent;
