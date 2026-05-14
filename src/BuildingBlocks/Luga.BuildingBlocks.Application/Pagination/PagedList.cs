namespace Luga.BuildingBlocks.Application.Pagination;

/// <summary>
/// Materialized page of <typeparamref name="T"/> items returned by list/query handlers.
/// Carries pagination metadata so the UI can render page selectors without an extra round-trip.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class PagedList<T>
{
    private PagedList(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    /// <summary>The items on the current page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Total items matching the query across all pages.</summary>
    public int TotalCount { get; }

    /// <summary>1-based page number actually returned (after normalization).</summary>
    public int Page { get; }

    /// <summary>Page size actually used (after normalization/clamping).</summary>
    public int PageSize { get; }

    /// <summary>Total number of pages for this query.</summary>
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>True when there is at least one page after the current one.</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>True when there is at least one page before the current one.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Builds a page from already-sliced items plus the total count.</summary>
    public static PagedList<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize) =>
        new(items, totalCount, Math.Max(1, page), Math.Max(0, pageSize));

    /// <summary>Builds a page from a <see cref="PagedRequest"/> after normalization.</summary>
    public static PagedList<T> FromRequest(IReadOnlyList<T> items, int totalCount, PagedRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        PagedRequest normalized = request.Normalized();
        return new PagedList<T>(items, totalCount, normalized.Page, normalized.PageSize);
    }

    /// <summary>Empty page (useful as a fallback / for tests).</summary>
    public static PagedList<T> Empty(int page = 1, int pageSize = 20) =>
        new([], 0, Math.Max(1, page), Math.Max(0, pageSize));

    /// <summary>Maps the items to a new type, preserving pagination metadata.</summary>
    public PagedList<TOut> Map<TOut>(Func<T, TOut> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        IReadOnlyList<TOut> mapped = [.. Items.Select(mapper)];
        return new PagedList<TOut>(mapped, TotalCount, Page, PageSize);
    }
}
