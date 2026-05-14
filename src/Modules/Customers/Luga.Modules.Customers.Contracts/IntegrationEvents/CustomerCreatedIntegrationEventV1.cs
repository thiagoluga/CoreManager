using Luga.BuildingBlocks.Domain.Events;
using Luga.BuildingBlocks.IntegrationEvents;

namespace Luga.Modules.Customers.Contracts.IntegrationEvents;

/// <summary>
/// Raised when a new <c>Customer</c> is created. Persisted via the outbox in the
/// same transaction as the entity insert (CLAUDE.md §7.17).
/// </summary>
public sealed record CustomerCreatedIntegrationEventV1(
    Guid CustomerId,
    Guid TenantId,
    string DisplayName,
    string Email,
    DateTime CreatedOn) : IIntegrationEvent, IDomainEvent
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    public int Version => 1;
}
