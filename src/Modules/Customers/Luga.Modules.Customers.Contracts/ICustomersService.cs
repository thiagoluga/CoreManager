using Luga.Modules.Customers.Contracts.DTOs;

namespace Luga.Modules.Customers.Contracts;

/// <summary>
/// In-process service consumed by other modules (Payments, Documents...).
/// Batch endpoints since day 1 (CLAUDE.md §3.4 perigo 2).
/// </summary>
public interface ICustomersService
{
    Task<CustomerContractDto?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerContractDto>> GetByIdsAsync(IEnumerable<Guid> customerIds, CancellationToken cancellationToken = default);
}
