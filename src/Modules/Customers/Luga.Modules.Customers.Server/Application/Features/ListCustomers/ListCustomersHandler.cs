using Luga.BuildingBlocks.Application.Pagination;
using Luga.BuildingBlocks.Application.Repositories;
using Luga.BuildingBlocks.Domain.Common;
using Luga.Modules.Customers.Server.Application.Mappers;
using Luga.Modules.Customers.Server.Domain.Entities;
using Luga.Modules.Customers.Shared.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Luga.Modules.Customers.Server.Application.Features.ListCustomers;

public sealed class ListCustomersHandler(IRepository<Customer> repository)
    : IRequestHandler<ListCustomersQuery, Result<PagedCustomersResponse>>
{
    private readonly IRepository<Customer> _repository = repository;

    public async Task<Result<PagedCustomersResponse>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        PagedRequest paging = new PagedRequest(request.Page, request.PageSize).Normalized();

        IQueryable<Customer> query = _repository.Query();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string term = request.Search.Trim();
            string termLower = term.ToLowerInvariant();
            query = query.Where(c =>
                EF.Functions.Like(c.DisplayName, $"%{term}%") ||
                EF.Functions.Like(c.Email, $"%{termLower}%"));
        }

        int total = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        List<Customer> rows = await query
            .OrderBy(c => c.DisplayName)
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        PagedList<CustomerSummaryDto> page = PagedList<CustomerSummaryDto>.Create(
            [.. rows.Select(CustomerMapper.ToSummary)],
            total,
            paging.Page,
            paging.PageSize);

        return Result.Success(new PagedCustomersResponse(
            Items: page.Items,
            TotalCount: page.TotalCount,
            Page: page.Page,
            PageSize: page.PageSize,
            TotalPages: page.TotalPages,
            HasNextPage: page.HasNextPage,
            HasPreviousPage: page.HasPreviousPage));
    }
}
