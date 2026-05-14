using FluentValidation;

namespace Luga.Modules.Customers.Server.Application.Features.CreateCustomer;

public sealed class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(c => c.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(c => c.Phone).MaximumLength(32);
        RuleFor(c => c.Document).MaximumLength(20);
        RuleFor(c => c.Notes).MaximumLength(4000);
    }
}
