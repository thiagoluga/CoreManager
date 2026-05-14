using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.GetCustomer;

public sealed record GetCustomerQuery(Guid Id) : IRequest<Result<CustomerDto>>;
