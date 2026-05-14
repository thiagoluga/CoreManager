using Luga.BuildingBlocks.Domain.Common;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest<Result>;
