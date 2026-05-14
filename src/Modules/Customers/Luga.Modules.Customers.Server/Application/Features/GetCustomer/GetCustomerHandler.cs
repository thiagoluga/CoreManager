using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Server.Application.Mappers;
using Luga.Modules.Customers.Server.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Server.Domain.Errors;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.GetCustomer;

public sealed class GetCustomerHandler(ICustomerRepository repository)
    : IRequestHandler<GetCustomerQuery, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<Result<CustomerDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Customer? customer = await _repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        return customer is null
            ? CustomersErrors.NotFound(request.Id)
            : CustomerMapper.ToDto(customer);
    }
}
