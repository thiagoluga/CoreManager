using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Core.Server.Application.Features.GetMyProfile;

/// <summary>
/// Returns the authenticated user's profile for the current tenant scope.
/// </summary>
public sealed record GetMyProfileQuery : IRequest<Result<TenantUserDto>>;
