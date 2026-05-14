using Luga.Modules.Customers.Contracts;
using Luga.Modules.Customers.Contracts.DTOs;
using Luga.Modules.Customers.Server.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;

namespace Luga.Modules.Customers.Server.Infrastructure.Services;

public sealed class CustomersService(ICustomerRepository repository) : ICustomersService
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<CustomerContractDto?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        Customer? customer = await _repository.GetByIdAsync(customerId, cancellationToken).ConfigureAwait(false);
        return customer is null ? null : Map(customer);
    }

    public async Task<IReadOnlyList<CustomerContractDto>> GetByIdsAsync(IEnumerable<Guid> customerIds, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Customer> customers =
            await _repository.GetByIdsAsync(customerIds, cancellationToken).ConfigureAwait(false);
        return [.. customers.Select(Map)];
    }

    private static CustomerContractDto Map(Customer c) =>
        new(c.Id, c.DisplayName, c.Email, c.Phone, c.IsActive);
}
