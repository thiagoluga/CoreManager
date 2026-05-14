using Luga.BuildingBlocks.IntegrationEvents;

namespace Luga.Modules.Core.Contracts.IntegrationEvents;

/// <summary>
/// Emitted when a new user is added to an existing tenant (invite accepted or
/// admin-created). Allows other modules to seed per-user state (e.g. default
/// permissions in Personalization).
/// </summary>
public sealed record TenantUserRegisteredIntegrationEventV1(
    Guid Id,
    DateTime OccurredOn,
    Guid UserId,
    Guid TenantId,
    string Username,
    string Role) : IIntegrationEvent
{
    /// <inheritdoc/>
    public int Version => 1;
}
