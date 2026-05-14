using Luga.Modules.Core.Contracts;
using Luga.Modules.Core.Contracts.DTOs;
using Luga.Modules.Core.Server.Application.Mappers;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;

namespace Luga.Modules.Core.Server.Infrastructure.Services;

/// <summary>
/// In-process <see cref="IUsersService"/> implementation.
/// </summary>
public sealed class UsersService(ITenantUserRepository users) : IUsersService
{
    private readonly ITenantUserRepository _users = users;

    /// <inheritdoc/>
    public async Task<TenantUserContractDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        TenantUser? user = await _users.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        return user?.ToContractDto();
    }

    /// <inheritdoc/>
    public async Task<TenantUserContractDto?> GetByUsernameAsync(
        Guid tenantId,
        string username,
        CancellationToken cancellationToken = default)
    {
        // Repository is scoped to the ambient tenant via global filter; tenantId on this call
        // is required for the future microservice mode where the receiver chooses the scope.
        TenantUser? user = await _users.GetByUsernameAsync(username, cancellationToken).ConfigureAwait(false);
        if (user is null || user.TenantId != tenantId)
        {
            return null;
        }

        return user.ToContractDto();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TenantUserContractDto>> GetByIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TenantUser> users = await _users.GetByIdsAsync(userIds, cancellationToken).ConfigureAwait(false);
        return [.. users.Select(u => u.ToContractDto())];
    }
}
