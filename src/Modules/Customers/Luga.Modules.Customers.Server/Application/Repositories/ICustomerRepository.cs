using Luga.BuildingBlocks.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;

namespace Luga.Modules.Customers.Server.Application.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<bool> EmailExistsAsync(string email, Guid? excludingId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
