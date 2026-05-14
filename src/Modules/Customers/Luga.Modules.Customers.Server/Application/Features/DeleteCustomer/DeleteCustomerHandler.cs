using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Server.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Server.Domain.Errors;
using Luga.Modules.Customers.Server.Infrastructure.Persistence;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.DeleteCustomer;

public sealed class DeleteCustomerHandler(
    ICustomerRepository repository,
    CustomersDbContext dbContext) : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly CustomersDbContext _dbContext = dbContext;

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Customer? customer = await _repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        if (customer is null)
        {
            return Result.Failure(CustomersErrors.NotFound(request.Id));
        }

        _repository.Remove(customer);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}
