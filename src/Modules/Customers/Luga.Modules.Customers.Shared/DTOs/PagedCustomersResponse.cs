namespace Luga.Modules.Customers.Shared.DTOs;

/// <summary>HTTP-friendly paged shape (mirrors BuildingBlocks.Application.PagedList).</summary>
public sealed record PagedCustomersResponse(
    IReadOnlyList<CustomerSummaryDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);
