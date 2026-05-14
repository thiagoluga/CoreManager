using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.CreateCustomer;

public sealed record CreateCustomerCommand(
    string DisplayName,
    string Email,
    string? Phone,
    string? Document,
    string? Notes,
    IReadOnlyDictionary<string, string>? CustomFields) : IRequest<Result<CustomerDto>>;
