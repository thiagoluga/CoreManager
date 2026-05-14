using Luga.BuildingBlocks.IntegrationEvents;

namespace Luga.Modules.Core.Contracts.IntegrationEvents;

/// <summary>
/// Emitted when a new tenant finishes the registration flow successfully.
/// Other modules (Marketing, Payments, Personalization) react to seed module-specific data.
/// </summary>
/// <remarks>
/// Versioned <c>V1</c> on the type name per CLAUDE.md §3.4 hazard 3 — breaking
/// changes must ship as <c>V2</c> alongside V1 until consumers migrate.
/// </remarks>
public sealed record TenantCreatedIntegrationEventV1(
    Guid Id,
    DateTime OccurredOn,
    Guid TenantId,
    string Slug,
    string Name,
    Guid OwnerUserId,
    string OwnerUsername,
    string DefaultCulture) : IIntegrationEvent
{
    /// <inheritdoc/>
    public int Version => 1;
}
