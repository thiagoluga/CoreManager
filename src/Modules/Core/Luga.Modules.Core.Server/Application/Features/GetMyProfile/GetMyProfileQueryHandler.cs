using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Core.Server.Application.Mappers;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Errors;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Core.Server.Application.Features.GetMyProfile;

/// <summary>
/// Returns the tenant user matching <see cref="ICurrentUser.UserId"/> together
/// with the permissions granted by the Personalization module. In the MVP the
/// permission list comes from <see cref="ICurrentUser.Permissions"/> directly;
/// once Personalization is wired it will look them up in the RBAC store.
/// </summary>
public sealed class GetMyProfileQueryHandler(
    ICurrentUser currentUser,
    ITenantUserRepository users) : IRequestHandler<GetMyProfileQuery, Result<TenantUserDto>>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ITenantUserRepository _users = users;

    public async Task<Result<TenantUserDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result.Failure<TenantUserDto>(CoreErrors.NotAuthenticated());
        }

        TenantUser? user = await _users.GetByIdAsync(_currentUser.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<TenantUserDto>(CoreErrors.UserNotFound(_currentUser.UserId));
        }

        IReadOnlyList<string> permissions = [.. _currentUser.Permissions];
        return Result.Success(user.ToDto(permissions));
    }
}
