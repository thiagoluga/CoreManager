namespace Luga.BuildingBlocks.Application.Pagination;

/// <summary>
/// Pagination input shared by list/query endpoints.
/// Page numbers are 1-based.
/// </summary>
/// <param name="Page">1-based page number. Defaults to 1.</param>
/// <param name="PageSize">Items per page. Defaults to 20. Clamped to <see cref="MaxPageSize"/>.</param>
public sealed record PagedRequest(int Page = 1, int PageSize = 20)
{
    /// <summary>Hard upper bound for <see cref="PageSize"/> to prevent pathological queries.</summary>
    public const int MaxPageSize = 200;

    /// <summary>Number of items to skip given the current page.</summary>
    public int Skip => (Math.Max(1, Page) - 1) * Math.Clamp(PageSize, 1, MaxPageSize);

    /// <summary>Number of items to take (alias of <see cref="PageSize"/>, clamped).</summary>
    public int Take => Math.Clamp(PageSize, 1, MaxPageSize);

    /// <summary>Normalizes input to safe bounds (page &gt;= 1, pageSize in [1, MaxPageSize]).</summary>
    public PagedRequest Normalized() => new(
        Page: Math.Max(1, Page),
        PageSize: Math.Clamp(PageSize, 1, MaxPageSize));
}
