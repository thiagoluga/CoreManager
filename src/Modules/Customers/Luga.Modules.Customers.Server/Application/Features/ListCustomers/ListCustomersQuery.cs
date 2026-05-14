using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

namespace Luga.Modules.Customers.Server.Application.Features.ListCustomers;

public sealed record ListCustomersQuery(int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PagedCustomersResponse>>;
