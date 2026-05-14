using Luga.Modules.Core.Shared.DTOs;

using Refit;

namespace Luga.Modules.Core.Shared.Refit;

/// <summary>
/// Refit HTTP client used by Core.Client to talk to the Core API.
/// Both Server (returns these shapes) and Client (sends/consumes them)
/// reference this interface — eliminating drift between them.
/// </summary>
public interface ICoreApi
{
    /// <summary>Public signup endpoint.</summary>
    [Post("/api/tenants/register")]
    Task<RegisterTenantResponse> RegisterTenantAsync(
        [Body] RegisterTenantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the tenant the current user belongs to.</summary>
    [Get("/api/tenants/current")]
    Task<TenantDto> GetCurrentTenantAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the authenticated user's profile.</summary>
    [Get("/api/users/me")]
    Task<TenantUserDto> GetMyProfileAsync(CancellationToken cancellationToken = default);
}
