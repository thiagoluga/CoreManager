namespace Luga.Modules.Core.Server.Domain.Enums;

/// <summary>Lifecycle states of a <c>TenantSubscription</c>.</summary>
public enum SubscriptionStatus
{
    Pending = 0,
    Active = 1,
    PastDue = 2,
    Suspended = 3,
    Cancelled = 4,
}
