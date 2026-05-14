using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid Id,
    string DisplayName,
    string Email,
    string? Phone,
    string? Document,
    string? Notes,
    bool IsActive,
    IReadOnlyDictionary<string, string>? CustomFields) : IRequest<Result<CustomerDto>>;
