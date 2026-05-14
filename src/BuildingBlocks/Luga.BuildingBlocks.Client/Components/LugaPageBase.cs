using Luga.BuildingBlocks.Client.Auth;
using Luga.BuildingBlocks.Client.Tenancy;

using Microsoft.AspNetCore.Components;

namespace Luga.BuildingBlocks.Client.Components;

/// <summary>
/// Base class for Luga pages. Exposes the ambient tenant / current user /
/// permission service as cascading parameters so pages can branch on them
/// without redundant <c>[Inject]</c> declarations (CLAUDE.md §9.4).
/// </summary>
public abstract class LugaPageBase : ComponentBase
{
    /// <summary>Tenant that the user is signed into.</summary>
    [CascadingParameter]
    protected TenantContext Tenant { get; set; } = TenantContext.Anonymous;

    /// <summary>Authenticated user (anonymous on public pages).</summary>
    [CascadingParameter]
    protected CurrentUser User { get; set; } = CurrentUser.Anonymous;

    /// <summary>Permission checker scoped to the current user/tenant.</summary>
    [CascadingParameter]
    protected IPermissionService Permissions { get; set; } = default!;

    /// <summary>Returns true when the user has the given permission code.</summary>
    protected bool HasPermission(string permission) =>
        Permissions is not null && Permissions.HasPermission(permission);

    /// <summary>Returns true when the given module is active in the tenant's subscription.</summary>
    protected bool HasModule(string moduleCode) => Tenant.HasModuleActive(moduleCode);
}
