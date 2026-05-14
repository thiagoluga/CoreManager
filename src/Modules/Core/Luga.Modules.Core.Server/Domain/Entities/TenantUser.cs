using Luga.BuildingBlocks.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Enums;

namespace Luga.Modules.Core.Server.Domain.Entities;

/// <summary>
/// A user belonging to a tenant. Multi-tenant — the row is keyed by the tenant
/// it operates in (the same email may exist in multiple tenants as separate users).
/// </summary>
public sealed class TenantUser : TenantEntity
{
    private TenantUser()
    {
        // EF Core
    }

    /// <summary>Email / login. Stored in lowercase for case-insensitive lookup.</summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>Human-friendly display name shown in the app bar and audit fields.</summary>
    public string DisplayName { get; private set; } = string.Empty;

    /// <summary>Baseline role inside the tenant (CLAUDE.md §7.3).</summary>
    public TenantUserRole Role { get; private set; } = TenantUserRole.Viewer;

    /// <summary>User-preferred UI culture (overrides the tenant default when set).</summary>
    public string PreferredCulture { get; private set; } = "pt-BR";

    /// <summary>Whether the user can sign in. Distinct from soft delete.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Creates the owner user for a newly-registered tenant.</summary>
    public static TenantUser CreateOwner(Guid tenantId, string email, string displayName, string preferredCulture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(preferredCulture);

        return new TenantUser
        {
            TenantId = tenantId,
            Username = email.Trim().ToLowerInvariant(),
            DisplayName = displayName.Trim(),
            Role = TenantUserRole.Owner,
            PreferredCulture = preferredCulture,
            IsActive = true,
        };
    }

    /// <summary>Creates a non-owner user invited into an existing tenant.</summary>
    public static TenantUser Invite(Guid tenantId, string email, string displayName, TenantUserRole role, string preferredCulture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(preferredCulture);

        if (role == TenantUserRole.Owner)
        {
            throw new InvalidOperationException("Use Tenant.Register to create the owner user.");
        }

        return new TenantUser
        {
            TenantId = tenantId,
            Username = email.Trim().ToLowerInvariant(),
            DisplayName = displayName.Trim(),
            Role = role,
            PreferredCulture = preferredCulture,
            IsActive = true,
        };
    }

    /// <summary>Updates display name and preferred culture on the profile screen.</summary>
    public void UpdateProfile(string displayName, string preferredCulture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(preferredCulture);

        DisplayName = displayName.Trim();
        PreferredCulture = preferredCulture;
    }

    /// <summary>Promotes / demotes the user (Owner is immutable here — transfer is a separate flow).</summary>
    public void ChangeRole(TenantUserRole role)
    {
        if (Role == TenantUserRole.Owner)
        {
            throw new InvalidOperationException("Owner role can only change through ownership transfer.");
        }

        if (role == TenantUserRole.Owner)
        {
            throw new InvalidOperationException("Owner is assigned via Tenant.Register / ownership transfer.");
        }

        Role = role;
    }

    /// <summary>Deactivates the user. Distinct from soft delete; the row stays for audit.</summary>
    public void Deactivate()
    {
        if (Role == TenantUserRole.Owner)
        {
            throw new InvalidOperationException("Cannot deactivate the tenant owner.");
        }

        IsActive = false;
    }

    /// <summary>Reactivates a previously deactivated user.</summary>
    public void Activate() => IsActive = true;
}
