using Luga.BuildingBlocks.Application.Pagination;

namespace Luga.Tests.BuildingBlocks.Application.Pagination;

public sealed class PagedListTests
{
    [Fact]
    public void Create_StoresItemsAndMetadata()
    {
        int[] items = [1, 2, 3];

        var page = PagedList<int>.Create(items, totalCount: 9, page: 2, pageSize: 3);

        page.Items.Should().Equal(items);
        page.TotalCount.Should().Be(9);
        page.Page.Should().Be(2);
        page.PageSize.Should().Be(3);
        page.TotalPages.Should().Be(3);
        page.HasPreviousPage.Should().BeTrue();
        page.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void FromRequest_AppliesNormalization()
    {
        PagedRequest request = new(Page: -5, PageSize: 999);

        var page = PagedList<string>.FromRequest(["a"], totalCount: 1, request);

        page.Page.Should().Be(1);
        page.PageSize.Should().Be(PagedRequest.MaxPageSize);
    }

    [Fact]
    public void Empty_HasZeroItemsAndZeroTotalPages()
    {
        var page = PagedList<int>.Empty();

        page.Items.Should().BeEmpty();
        page.TotalCount.Should().Be(0);
        page.TotalPages.Should().Be(0);
        page.HasNextPage.Should().BeFalse();
        page.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void FirstPage_HasNoPreviousButHasNext()
    {
        var page = PagedList<int>.Create([1, 2], totalCount: 10, page: 1, pageSize: 2);

        page.HasPreviousPage.Should().BeFalse();
        page.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void LastPage_HasPreviousButNoNext()
    {
        var page = PagedList<int>.Create([9, 10], totalCount: 10, page: 5, pageSize: 2);

        page.HasPreviousPage.Should().BeTrue();
        page.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Map_TransformsItemsButKeepsMetadata()
    {
        var source = PagedList<int>.Create([1, 2, 3], totalCount: 30, page: 2, pageSize: 3);

        PagedList<string> mapped = source.Map(i => i.ToString(System.Globalization.CultureInfo.InvariantCulture));

        mapped.Items.Should().Equal("1", "2", "3");
        mapped.TotalCount.Should().Be(30);
        mapped.Page.Should().Be(2);
        mapped.PageSize.Should().Be(3);
    }
}
