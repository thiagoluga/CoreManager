using Luga.BuildingBlocks.Domain.Common;

namespace Luga.Modules.Customers.Server.Domain.Errors;

public static class CustomersErrors
{
    public static readonly Error EmailAlreadyExists =
        new("Customer.EmailAlreadyExists", "Já existe um customer com esse e-mail.");

    public static Error NotFound(Guid id) =>
        new("Customer.NotFound", $"Customer com id '{id}' não foi encontrado.");
}
