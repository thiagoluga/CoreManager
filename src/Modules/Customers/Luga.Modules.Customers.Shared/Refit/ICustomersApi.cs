using Luga.Modules.Customers.Shared.DTOs;

using Refit;

namespace Luga.Modules.Customers.Shared.Refit;

public interface ICustomersApi
{
    [Get("/api/customers")]
    Task<PagedCustomersResponse> ListAsync(
        [Query] int page = 1,
        [Query] int pageSize = 20,
        [Query] string? search = null,
        CancellationToken cancellationToken = default);

    [Get("/api/customers/{id}")]
    Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    [Post("/api/customers")]
    Task<CustomerDto> CreateAsync([Body] CreateCustomerRequest request, CancellationToken cancellationToken = default);

    [Put("/api/customers/{id}")]
    Task<CustomerDto> UpdateAsync(Guid id, [Body] UpdateCustomerRequest request, CancellationToken cancellationToken = default);

    [Delete("/api/customers/{id}")]
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
