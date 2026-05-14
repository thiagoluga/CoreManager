using FluentValidation;

namespace Luga.Modules.Marketing.Server.Application;

public sealed class SubmitContactValidator : AbstractValidator<SubmitContactCommand>
{
    public SubmitContactValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Company).MaximumLength(120);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
    }
}
