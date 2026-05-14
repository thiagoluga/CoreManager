using Luga.BuildingBlocks.Application.Abstractions;
using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Core.Server.Application.Repositories;
using Luga.Modules.Core.Server.Domain.Entities;
using Luga.Modules.Core.Server.Domain.Errors;
using Luga.Modules.Core.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Core.Server.Application.Features.RegisterTenant;

/// <summary>
/// Handler for <see cref="RegisterTenantCommand"/>. Creates the tenant + owner
/// in one unit of work; the <c>DomainEventToOutboxInterceptor</c> takes care of
/// flushing the integration event to the outbox in the same transaction.
/// </summary>
public sealed class RegisterTenantCommandHandler(
    ITenantRepository tenants,
    ITenantUserRepository users,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<RegisterTenantCommand, Result<RegisterTenantResponse>>
{
    private readonly ITenantRepository _tenants = tenants;
    private readonly ITenantUserRepository _users = users;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<RegisterTenantResponse>> Handle(
        RegisterTenantCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        string slug = command.TenantSlug.Trim().ToLowerInvariant();
        if (await _tenants.SlugExistsAsync(slug, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<RegisterTenantResponse>(CoreErrors.SlugTaken(slug));
        }

        (Tenant tenant, TenantUser owner) = Tenant.Register(
            name: command.TenantName,
            slug: slug,
            ownerEmail: command.OwnerEmail,
            ownerDisplayName: command.OwnerDisplayName,
            defaultCulture: command.DefaultCulture,
            timeProvider: _timeProvider);

        _tenants.Add(tenant);
        _users.Add(owner);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(new RegisterTenantResponse(
            TenantId: tenant.Id,
            Slug: tenant.Slug,
            OwnerUserId: owner.Id));
    }
}
