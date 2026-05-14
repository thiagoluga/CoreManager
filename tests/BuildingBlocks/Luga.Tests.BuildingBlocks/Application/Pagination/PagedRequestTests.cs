using Luga.BuildingBlocks.Application.Pagination;

namespace Luga.Tests.BuildingBlocks.Application.Pagination;

public sealed class PagedRequestTests
{
    [Fact]
    public void Defaults_AreSensible()
    {
        PagedRequest request = new();

        request.Page.Should().Be(1);
        request.PageSize.Should().Be(20);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(1, 1)]
    [InlineData(7, 7)]
    public void Normalized_ClampsPageToAtLeastOne(int input, int expected)
    {
        PagedRequest request = new(Page: input, PageSize: 10);

        request.Normalized().Page.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(50, 50)]
    [InlineData(PagedRequest.MaxPageSize + 100, PagedRequest.MaxPageSize)]
    public void Normalized_ClampsPageSizeWithinRange(int input, int expected)
    {
        PagedRequest request = new(Page: 1, PageSize: input);

        request.Normalized().PageSize.Should().Be(expected);
    }

    [Fact]
    public void Skip_ComputedFromPageAndSize()
    {
        PagedRequest request = new(Page: 3, PageSize: 20);

        request.Skip.Should().Be(40);
    }

    [Fact]
    public void Take_ClampedToMaxPageSize()
    {
        PagedRequest request = new(Page: 1, PageSize: PagedRequest.MaxPageSize + 10);

        request.Take.Should().Be(PagedRequest.MaxPageSize);
    }
}
