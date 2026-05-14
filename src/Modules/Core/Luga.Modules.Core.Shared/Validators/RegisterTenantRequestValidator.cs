using FluentValidation;

using Luga.Modules.Core.Shared.DTOs;

namespace Luga.Modules.Core.Shared.Validators;

/// <summary>
/// FluentValidation rules for the public tenant signup payload. Shared with the
/// Blazor form (EditForm) and with the API handler (ValidationBehavior in MediatR).
/// </summary>
public sealed class RegisterTenantRequestValidator : AbstractValidator<RegisterTenantRequest>
{
    public RegisterTenantRequestValidator()
    {
        RuleFor(r => r.TenantName)
            .NotEmpty().WithMessage("Tenant name is required.")
            .MaximumLength(120);

        RuleFor(r => r.TenantSlug)
            .NotEmpty().WithMessage("Tenant slug is required.")
            .MinimumLength(3)
            .MaximumLength(60)
            .Matches("^[a-z0-9][a-z0-9-]*$")
            .WithMessage("Slug must be lowercase alphanumeric with hyphens.");

        RuleFor(r => r.OwnerEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.OwnerDisplayName)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(r => r.DefaultCulture)
            .NotEmpty()
            .Matches("^[a-z]{2}-[A-Z]{2}$")
            .WithMessage("Culture must follow the {language}-{region} pattern (e.g. pt-BR).");
    }
}
