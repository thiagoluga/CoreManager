using Luga.BuildingBlocks.Infrastructure.Persistence.Repositories;
using Luga.Modules.Customers.Server.Application.Repositories;
using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Server.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Customers.Server.Infrastructure.Repositories;

public sealed class CustomerRepository(CustomersDbContext context) : Repository<Customer>(context), ICustomerRepository
{
    public Task<bool> EmailExistsAsync(string email, Guid? excludingId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        string normalized = email.Trim().ToLowerInvariant();
        return DbSet.AnyAsync(
            c => c.Email == normalized && (excludingId == null || c.Id != excludingId),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        Guid[] idArray = [.. ids];
        return await DbSet
            .Where(c => idArray.Contains(c.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
