using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Server.Application.Mappers;
using Luga.Modules.Customers.Server.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Server.Domain.Errors;
using Luga.Modules.Customers.Server.Infrastructure.Persistence;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.CreateCustomer;

public sealed class CreateCustomerHandler(
    ICustomerRepository repository,
    CustomersDbContext dbContext) : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly CustomersDbContext _dbContext = dbContext;

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        bool emailTaken = await _repository.EmailExistsAsync(request.Email, excludingId: null, cancellationToken).ConfigureAwait(false);
        if (emailTaken)
        {
            return CustomersErrors.EmailAlreadyExists;
        }

        IDictionary<string, string>? customFields = request.CustomFields is null
            ? null
            : new Dictionary<string, string>(request.CustomFields, StringComparer.Ordinal);

        Customer customer = Customer.Create(
            request.DisplayName,
            request.Email,
            request.Phone,
            request.Document,
            request.Notes,
            customFields);

        _repository.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return CustomerMapper.ToDto(customer);
    }
}
