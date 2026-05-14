using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Server.Application.Mappers;
using Luga.Modules.Customers.Server.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Server.Domain.Errors;
using Luga.Modules.Customers.Server.Infrastructure.Persistence;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.UpdateCustomer;

public sealed class UpdateCustomerHandler(
    ICustomerRepository repository,
    CustomersDbContext dbContext) : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly CustomersDbContext _dbContext = dbContext;

    public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Customer? customer = await _repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        if (customer is null)
        {
            return CustomersErrors.NotFound(request.Id);
        }

        bool emailTaken = await _repository
            .EmailExistsAsync(request.Email, excludingId: request.Id, cancellationToken)
            .ConfigureAwait(false);
        if (emailTaken)
        {
            return CustomersErrors.EmailAlreadyExists;
        }

        customer.DisplayName = request.DisplayName.Trim();
        customer.Email = request.Email.Trim().ToLowerInvariant();
        customer.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        customer.Document = string.IsNullOrWhiteSpace(request.Document) ? null : request.Document.Trim();
        customer.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        customer.IsActive = request.IsActive;

        if (request.CustomFields is not null)
        {
            customer.CustomFields = new Dictionary<string, string>(request.CustomFields, StringComparer.Ordinal);
        }

        _repository.Update(customer);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return CustomerMapper.ToDto(customer);
    }
}
