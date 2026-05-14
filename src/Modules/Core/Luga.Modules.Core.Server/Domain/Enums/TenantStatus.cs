namespace Luga.Modules.Core.Server.Domain.Enums;

/// <summary>
/// Lifecycle state of a tenant.
/// </summary>
public enum TenantStatus
{
    /// <summary>Active and able to use the platform.</summary>
    Active = 0,

    /// <summary>Temporarily suspended (e.g. delinquent). API blocks mutations.</summary>
    Suspended = 1,

    /// <summary>Cancelled by the customer. Soft-deleted shortly after.</summary>
    Cancelled = 2,
}
