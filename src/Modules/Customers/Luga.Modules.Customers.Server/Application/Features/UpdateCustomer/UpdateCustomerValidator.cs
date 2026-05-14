using FluentValidation;

namespace Luga.Modules.Customers.Server.Application.Features.UpdateCustomer;

public sealed class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(c => c.Phone).MaximumLength(32);
        RuleFor(c => c.Document).MaximumLength(20);
        RuleFor(c => c.Notes).MaximumLength(4000);
    }
}
