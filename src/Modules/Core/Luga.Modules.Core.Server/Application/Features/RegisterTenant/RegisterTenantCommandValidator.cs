using FluentValidation;

namespace Luga.Modules.Core.Server.Application.Features.RegisterTenant;

/// <summary>
/// Mirror of <c>RegisterTenantRequestValidator</c> on the Server side so the
/// MediatR <c>ValidationBehavior</c> picks it up. Mirrors the shared rules
/// verbatim — could be merged in the future via a common rule library.
/// </summary>
public sealed class RegisterTenantCommandValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantCommandValidator()
    {
        RuleFor(c => c.TenantName).NotEmpty().MaximumLength(120);
        RuleFor(c => c.TenantSlug)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(60)
            .Matches("^[a-z0-9][a-z0-9-]*$");
        RuleFor(c => c.OwnerEmail).NotEmpty().EmailAddress();
        RuleFor(c => c.OwnerDisplayName).NotEmpty().MaximumLength(120);
        RuleFor(c => c.DefaultCulture).NotEmpty().Matches("^[a-z]{2}-[A-Z]{2}$");
    }
}
